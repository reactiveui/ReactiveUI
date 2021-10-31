// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ReactiveUI;

/// <summary> takes a batch of updates in their natural order (i.e. the order they occurred in the client code) and normalizes them to
/// something iOS can consume when performing batch updates to a table or collection view
/// iOS requires that all deletes be specified first with indexes relative to the source data *before* any insertions are applied
/// it then requires insertions be specified next relative to the source data *after* any deletions are applied
/// this code also de-duplicates as necessary. The simplest possible scenario for this is adding and immediately deleting an
/// item. iOS should never even be told about this set of updates because they cancel each other out.
/// </summary>
public static class IndexNormalizer
{
    /// <summary>
    /// Normalizes the specified updates.
    /// </summary>
    /// <param name="updates">The updates.</param>
    /// <returns>A list updates.</returns>
    public static IList<Update?> Normalize(IEnumerable<Update> updates)
    {
        var updatesList = updates.ToList();
        MarkDuplicates(updatesList);

        return updatesList
               .Select((x, i) => x.IsDuplicate ? null : Update.Create(x.Type, CalculateUpdateIndex(updatesList, i)))
               .Where(x => x is not null)
               .ToList();
    }

    // find all updates that cancel each other out, and mark them as duplicates
    // they're still required for subsequent index calculations, but ultimately they won't be returned from the Normalize method
    private static void MarkDuplicates(IList<Update> updates)
    {
        for (var updateIndex = 1; updateIndex < updates.Count; ++updateIndex)
        {
            var update = updates[updateIndex];

            if (update.Type == UpdateType.Delete)
            {
                var deletionIndex = update.Index;

                for (var i = 0; i < updateIndex; ++i)
                {
                    var priorUpdate = updates[i];

                    if (priorUpdate.Type != UpdateType.Add || priorUpdate.IsDuplicate)
                    {
                        continue;
                    }

                    var additionDataIndex = CalculateAdditionIndex(updates, 0, updateIndex, i);

                    if (deletionIndex == additionDataIndex)
                    {
                        updates[i] = priorUpdate.AsDuplicate();
                        updates[updateIndex] = update.AsDuplicate();

                        break;
                    }
                }
            }
        }
    }

    // calculate the index for an update
    private static int CalculateUpdateIndex(IList<Update> updates, int updateIndex) =>
        updates[updateIndex].Type switch
        {
            UpdateType.Add =>
                CalculateAdditionIndex(updates, 0, updates.Count, updateIndex),
            UpdateType.Delete =>
                CalculateDeletionIndex(updates, 0, updateIndex, updates[updateIndex].Index),
            _ => throw new NotSupportedException(),
        };

    // calculate the index for an addition update
    // the formula is:
    //   Ia = Io + Na - Nd
    // where:
    //   Ia = addition index
    //   Io = the addition's original index (as specified by client code)
    //   Na = the number of subsequent addition updates whose original index is <= the running (calculated) index of the update whose index is being calculated
    //   Nd = the number of subsequent deletion updates whose original index is < the running (calculated) index of the update whose index is being calculated
    private static int CalculateAdditionIndex(IList<Update> updates, int start, int count, int updateIndex)
    {
        var update = updates[updateIndex];
        Debug.Assert(update.Type == UpdateType.Add, "Must be adding items");
        var originalIndex = update.Index;
        var runningCalculation = originalIndex;

        for (var i = updateIndex + 1; i < start + count; ++i)
        {
            var subsequentUpdate = updates[i];

            if (subsequentUpdate.Type == UpdateType.Add)
            {
                if (subsequentUpdate.Index <= runningCalculation)
                {
                    ++runningCalculation;
                }

                continue;
            }
            else if (subsequentUpdate.Type == UpdateType.Delete)
            {
                if (subsequentUpdate.Index < runningCalculation)
                {
                    --runningCalculation;
                }

                continue;
            }
        }

        return runningCalculation;
    }

    // calculate the index for a deletion update
    // the formula is:
    //    Id = Io + Nd - Na
    // where:
    //    Id = deletion index
    //    Io = the deletion's original index (as specified by client code)
    //    Nd = the number of prior deletion updates whose original index is <= the running (calculated) index of the update whose index is being calculated
    //    Na = the number of prior addition updates whose original index is <= the running (calculated) index of the update whose index is being calculated
    private static int CalculateDeletionIndex(IList<Update> updates, int start, int deletionIndex, int originalIndex)
    {
        var runningCalculation = originalIndex;

        for (var i = deletionIndex - 1; i >= start; --i)
        {
            var priorUpdate = updates[i];

            if (priorUpdate.Type == UpdateType.Delete)
            {
                if (priorUpdate.Index <= runningCalculation)
                {
                    ++runningCalculation;
                }

                continue;
            }
            else if (priorUpdate.Type == UpdateType.Add)
            {
                if (priorUpdate.Index <= runningCalculation)
                {
                    --runningCalculation;
                }

                continue;
            }
        }

        return runningCalculation;
    }
}