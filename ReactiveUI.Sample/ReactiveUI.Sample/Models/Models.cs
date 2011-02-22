using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ReactiveUI.Sample.Models
{
    public class AppModel : ReactiveValidatedObject
    {
        public ReactiveCollection<BlockItem> CompletedBlocks { get; protected set; }

        public AppModel()
        {
            CompletedBlocks = new ReactiveCollection<BlockItem>();
        }
    }

    public class BlockItem : ReactiveValidatedObject
    {
        [Required]
        public string Description { get; set;  }

        public DateTimeOffset? StartedAt { get; set; }

        public DateTimeOffset? EndedAt { get; set; }

        public ReactiveCollection<PauseRecord> PauseList { get; protected set; }

        public BlockItem()
        {
            PauseList = new ReactiveCollection<PauseRecord>();
        }

        public IEnumerable<PauseRecord> GetPausesDuringBlock(DateTimeOffset? NewerThan = null, DateTimeOffset? OlderThan = null, int? Limit = null)
        {
            OlderThan = OlderThan ?? DateTimeOffset.MaxValue;
            NewerThan = NewerThan ?? DateTimeOffset.MinValue;
            Limit = Limit ?? Int32.MaxValue;

            return PauseList
                .Where(x => x.StartedAt < OlderThan.Value && x.StartedAt > NewerThan.Value)
                .Take(Limit.Value).ToArray();
        }

        public void AddRecordOfPause(PauseRecord PauseDuration)
        {
            PauseList.Add(PauseDuration);
        }
    }

    public class PauseRecord : ReactiveValidatedObject
    {
        DateTimeOffset _StartedAt;
        [Required]
        public DateTimeOffset StartedAt { 
            get { return _StartedAt; }
            set { this.RaiseAndSetIfChanged(x => x.StartedAt, value); }
        }

        DateTimeOffset _EndedAt;
        [Required]
        public DateTimeOffset EndedAt {
            get { return _EndedAt; }
            set { this.RaiseAndSetIfChanged(x => x.EndedAt, value); }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :