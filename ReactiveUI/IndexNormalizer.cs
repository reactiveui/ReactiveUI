namespace ReactiveUI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public enum UpdateType
    {
        Add,
        Delete
    }

    public sealed class Update
    {
        private readonly UpdateType type;
        private readonly int index;
        private int? duplicateCounterpartIndex;

        private Update(UpdateType type, int index)
        {
            this.type = type;
            this.index = index;
        }

        public UpdateType Type
        {
            get { return this.type; }
        }

        public int Index
        {
            get { return this.index; }
        }

        public int? DuplicateCounterpartIndex
        {
            get { return this.duplicateCounterpartIndex; }
            set { this.duplicateCounterpartIndex = value; }
        }

        public bool IsDuplicate
        {
            get { return this.duplicateCounterpartIndex.HasValue; }
        }

        public override string ToString()
        {
            return this.type.ToString()[0] + this.index.ToString();
        }

        public static Update Create(UpdateType type, int index)
        {
            return new Update(type, index);
        }

        public static Update CreateAdd(int index)
        {
            return new Update(UpdateType.Add, index);
        }

        public static Update CreateDelete(int index)
        {
            return new Update(UpdateType.Delete, index);
        }
    }

    public static class IndexNormalizer
    {
        public static IList<Update> Normalize(IList<Update> updates)
        {
            var results = new List<Update>();

            for (var updateIndex = 0; updateIndex < updates.Count; ++updateIndex)
            {
                var update = updates[updateIndex];

                if (update.Type == UpdateType.Delete)
                {
                    var deletionIndex = update.Index;

                    for (var i = 0; i < results.Count; ++i)
                    {
                        var priorUpdate = results[i];

                        if (priorUpdate.Type != UpdateType.Add || priorUpdate.IsDuplicate)
                        {
                            continue;
                        }

                        var additionDataIndex = CalculateAdditionIndex(results, 0, results.Count, i);

                        if (deletionIndex == additionDataIndex)
                        {
                            priorUpdate.DuplicateCounterpartIndex = updateIndex;
                            update.DuplicateCounterpartIndex = i;
                            break;
                        }
                    }
                }

                results.Add(update);
            }

            results = results
                .Select((x, i) => x.IsDuplicate ? null : Update.Create(x.Type, CalculateUpdateIndex(results, 0, results.Count, i)))
                .Where(x => x != null)
                .ToList();

            return results;
        }

        private static int CalculateUpdateIndex(IList<Update> updates, int start, int count, int updateIndex)
        {
            switch (updates[updateIndex].Type)
            {
                case UpdateType.Add:
                    return CalculateAdditionIndex(updates, start, count, updateIndex);
                case UpdateType.Delete:
                    return CalculateDeletionIndex(updates, start, count, updateIndex, updates[updateIndex].Index);
                default:
                    throw new NotSupportedException();
            }
        }

        private static int CalculateAdditionIndex(IList<Update> updates, int start, int count, int updateIndex)
        {
            var update = updates[updateIndex];
            Debug.Assert(update.Type == UpdateType.Add);
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

        private static int CalculateDeletionIndex(IList<Update> updates, int start, int count, int deletionIndex, int originalIndex)
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
}