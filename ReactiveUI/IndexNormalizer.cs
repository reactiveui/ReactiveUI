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
        private readonly bool isDuplicate;

        private Update(UpdateType type, int index, bool isDuplicate = false)
        {
            this.type = type;
            this.index = index;
            this.isDuplicate = isDuplicate;
        }

        public UpdateType Type
        {
            get { return this.type; }
        }

        public int Index
        {
            get { return this.index; }
        }

        public bool IsDuplicate
        {
            get { return this.isDuplicate; }
        }

        public override string ToString()
        {
            return this.type.ToString()[0] + this.index.ToString();
        }

        public Update AsDuplicate()
        {
            return new Update(this.type, this.index, isDuplicate: true);
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
        public static IList<Update> Normalize(IEnumerable<Update> updates)
        {
            var updatesList = updates.ToList();
            MarkDuplicates(updatesList);

            return updatesList
                .Select((x, i) => x.IsDuplicate ? null : Update.Create(x.Type, CalculateUpdateIndex(updatesList, i)))
                .Where(x => x != null)
                .ToList();
        }

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

        private static int CalculateUpdateIndex(IList<Update> updates, int updateIndex)
        {
            switch (updates[updateIndex].Type)
            {
                case UpdateType.Add:
                    return CalculateAdditionIndex(updates, 0, updates.Count, updateIndex);
                case UpdateType.Delete:
                    return CalculateDeletionIndex(updates, 0, updates.Count, updateIndex, updates[updateIndex].Index);
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