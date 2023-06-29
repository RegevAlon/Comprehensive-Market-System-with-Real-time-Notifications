using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.RepoLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class EventManager
    {
        private int _shopId;
        private ConcurrentDictionary<string, SynchronizedCollection<Member>> _listeners;

        public ConcurrentDictionary<string, SynchronizedCollection<Member>> Listeners { get => _listeners; set => _listeners = value; }

        public EventManager(int shopId)
        {
            _shopId = shopId;
            _listeners = new ConcurrentDictionary<string, SynchronizedCollection<Member>>();
            _listeners.TryAdd("Product Sell Event", new SynchronizedCollection<Member>());
            _listeners.TryAdd("Remove Appointment Event", new SynchronizedCollection<Member>());
            _listeners.TryAdd("Shop Closed Event", new SynchronizedCollection<Member>());
            _listeners.TryAdd("Shop Open Event", new SynchronizedCollection<Member>());
            _listeners.TryAdd("Report Event", new SynchronizedCollection<Member>());
            _listeners.TryAdd("Message Event", new SynchronizedCollection<Member>());
            _listeners.TryAdd("Add Appointment Event", new SynchronizedCollection<Member>());
            _listeners.TryAdd("Product Bid Event", new SynchronizedCollection<Member>());
            _listeners.TryAdd("Product Counter Bid Event", new SynchronizedCollection<Member>());
            _listeners.TryAdd("Pending Agreement Event", new SynchronizedCollection<Member>());
            _listeners.TryAdd("Counter Bid Declined Event", new SynchronizedCollection<Member>());
            UploadEventsFromContext();
        }

        private void UploadEventsFromContext()
        {
            MarketContext context = MarketContext.GetInstance();
            List<EventDTO> events =  context.Events.Where((e) => e.ShopId == _shopId).ToList();
            List<MemberDTO> members = context.Members.Where((m)=>m.IsSystemAdmin==true).ToList();
            foreach(EventDTO e in events)
            {
                _listeners[e.Name].Add(MemberRepo.GetInstance().GetById(e.Listener.Id));
            }
            foreach(MemberDTO m in members)
            {
                Member member = MemberRepo.GetInstance().GetById(m.Id);
                if (!_listeners["Report Event"].Contains(member))
                    _listeners["Report Event"].Add(member);
            }
        }

        public void Subscribe(Member user, Event e)
        {
            if (!_listeners[e.Name].Contains(user))
            {
                _listeners[e.Name].Add(user);
                MarketContext.GetInstance().Events.Add(new EventDTO(e.Name, _shopId, MarketContext.GetInstance().Members.Find(user.Id)));
                MarketContext.GetInstance().SaveChanges();
            }
            else throw new Exception("User already sign to this event.");
        }
        public void Unsubscribe(Member user, Event e)
        {
            if (_listeners[e.Name].Contains(user))
            {
                _listeners[e.Name].Remove(user);
                EventDTO eventDTO = MarketContext.GetInstance().Events
                    .Where((e)=>e.Listener.Id == user.Id && e.ShopId==_shopId).FirstOrDefault();
                MarketContext.GetInstance().Events.Remove(eventDTO);
                MarketContext.GetInstance().SaveChanges();
            }
            else throw new Exception("User already not signed to this event.");

        }

        public void UnsubscribeToAll(Member user)
        {
            foreach (string eventName in _listeners.Keys)
                if (_listeners[eventName].Contains(user))
                {
                    _listeners[eventName].Remove(user);
                    EventDTO eventDTO = MarketContext.GetInstance().Events
                        .Where((e) => e.Listener.Id == user.Id && e.ShopId == _shopId).FirstOrDefault();
                    MarketContext.GetInstance().Events.Remove(eventDTO);
                }
            MarketContext.GetInstance().SaveChanges();
        }
        public void NotifySubscribers(Event e)
        {
            foreach (Member user in _listeners[e.Name])
            {
                e.Update(user);
            }
        }

        public void SubscribeToAll(Member user)
        {
            List<string> keys = _listeners.Keys.ToList<string>();
            keys.Remove("Pending Agreement Event");
            foreach (string eventName in keys)
            {
                _listeners[eventName].Add(user);
                MarketContext.GetInstance().Events.Add(new EventDTO(eventName, _shopId, MarketContext.GetInstance().Members.Find(user.Id)));
                MarketContext.GetInstance().SaveChanges();
            }
        }
        public void SubscribeReportEvent(Member user)
        {
            _listeners["Report Event"].Add(user);
            MarketContext.GetInstance().Events.Add(new EventDTO("Report Event", _shopId, MarketContext.GetInstance().Members.Find(user.Id)));
            MarketContext.GetInstance().SaveChanges();
        }
        public void SubscribePendingAgreementEvent(Member user)
        {
            _listeners["Pending Agreement Event"].Add(user);
            MarketContext.GetInstance().Events.Add(new EventDTO("Pending Agreement Event", _shopId, MarketContext.GetInstance().Members.Find(user.Id)));
            MarketContext.GetInstance().SaveChanges();
        }
    }
}
