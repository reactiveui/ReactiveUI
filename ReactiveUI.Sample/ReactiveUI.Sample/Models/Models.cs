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


    /* COOLSTUFF: Data Validation
     *
     * In addition to providing change notification support,
     * ReactiveObservableObject also provides validation support via Data
     * Annotations. For every property you want to limit, you can add
     * special attributes from the System.ComponentModel.DataAnnotations
     * namespace (or derive from ValidationAttribute and create your own)
     *
     * To let WPF know about the validation, we implement IDataErrorInfo,
     * which WPF will use to figure out whether a property is valid. The
     * IsValid function will tell us whether all of the properties are
     * valid.
     */

    public class BlockItem : ReactiveValidatedObject
    {
        [Required]
        public string Description { get; set;  }

        [Required]
        public DateTimeOffset? StartedAt { get; set; }

        [Required]
        public DateTimeOffset? EndedAt { get; set; }

        public ReactiveCollection<PauseRecord> PauseList { get; protected set; }

        public BlockItem()
        {
            PauseList = new ReactiveCollection<PauseRecord>();
        }

        public IEnumerable<PauseRecord> GetPausesDuringBlock(
            DateTimeOffset? newerThan = null, 
            DateTimeOffset? olderThan = null, 
            int? limit = null)
        {
            olderThan = olderThan ?? DateTimeOffset.MaxValue;
            newerThan = newerThan ?? DateTimeOffset.MinValue;
            limit = limit ?? Int32.MaxValue;

            return PauseList
                .Where(x => x.StartedAt < olderThan.Value && x.StartedAt > newerThan.Value)
                .Take(limit.Value).ToArray();
        }

        public void AddRecordOfPause(PauseRecord pauseDuration)
        {
            PauseList.Add(pauseDuration);
        }
    }


    /* COOLSTUFF: Read-write properties
     *
     * This class shows how to declare read-write properties in ReactiveUI -
     * when another part of code changes these properties, the View will be
     * notified (INotifyPropertyChanged), as well as anyone subscribing to these
     * properties 
     *
     * One thing that's important to know, is that the backing field here *must*
     * be named _StartedAt for the property StartedAt. If you don't use this
     * convention for your application, you can override it via 
     * RxApp.GetFieldNameForPropertyNameFunc.
     */

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

// vim: tw=120 ts=4 sw=4 et :
