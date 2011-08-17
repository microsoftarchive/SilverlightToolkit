// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace PhoneToolkitSample.Data
{
    public class ConversationObject : ObservableCollection<EmailObject>, INotifyPropertyChanged
    {
        public string Title { get; set; }

        public string ContentInfo { get; set; }

        private EmailObject _lastMessageReceived;

        public EmailObject LastMessageReceived
        {
            get
            {
                return _lastMessageReceived;
            }
            set
            {
                _lastMessageReceived = value;
                NotifyPropertyChanged("LastMessageReceived");
            }
        }

        private bool _hasMultipleMessages;

        public bool HasSingleMessage
        {
            get
            {
                return _hasMultipleMessages;
            }
            set
            {
                _hasMultipleMessages = value;
                NotifyPropertyChanged("HasSingleMessage");
            }
        }

        public ConversationObject(string ci)
            : base()
        {
            ContentInfo = ci;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            LastMessageReceived = this.Items[0];

            HasSingleMessage = (this.Items.Count > 1) ? false : true;
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
