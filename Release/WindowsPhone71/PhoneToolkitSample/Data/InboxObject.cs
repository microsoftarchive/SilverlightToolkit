// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;

namespace PhoneToolkitSample.Data
{
    public class InboxObject : ObservableCollection<ConversationObject>
    {
        public InboxObject()
            : base()
        {
            ConversationObject one = new ConversationObject("3 messages, 0 unread");
            one.Add(new EmailObject("Anne Wallace", "Where are we going for lunch today?", "Sure! We haven't seen him in a while. I'm sure there are lots of things to catch up on."));
            one.Add(new EmailObject("Me", "Where are we going for lunch today?", "I down for it. Should we invite Dave to go grab lunch with us today?"));
            one.Add(new EmailObject("Bruno Denuit", "Where are we going for lunch today?", "I vote for the Thai restaurant across the street. I always forget the name."));

            ConversationObject two = new ConversationObject("3 messages, 0 unread");
            two.Add(new EmailObject("Adriana Giorgi", "Did you have fun on your trip?", "It'so awesome that you got the chance to go ..."));

            ConversationObject three = new ConversationObject("4 messages, 1 unread");
            three.Add(new EmailObject("Belinda Newman", "hawaii pictures are up!", "This is awesome. It's great to see that you guys had a good time."));
            three.Add(new EmailObject("Richard Carey", "hawaii pictures are up!", "Oh man, that picture of you with the fish is amazing."));
            three.Add(new EmailObject("Christof Sprenger", "hawaii pictures are up!", "Impressive stuff. I love it out there, and can't wait to get back."));
            three.Add(new EmailObject("Melissa Kerr", "hawaii pictures are up!", "Check them out. You'll have to tag yourselves in there 'cause I'm lazy. ;)"));

            Add(one);
            Add(two);
            Add(three);
        }
    }
}
