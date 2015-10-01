using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SupportingCodeForTalk
{
    class Program
    {
        static void Main(string[] args)
        {            
            TestScheduler scheduler = new TestScheduler();

            scheduler.AdvanceTo(DateTime.Now.Ticks);

            //IScheduler scheduler = Scheduler.Default;

            var heartbeats = Observable.Generate(
                0, 
                i => i < 15,
                i => i + 1, 
                _ => new Heartbeat(),
                i => i == 3 || i == 6 ? TimeSpan.FromSeconds(5) : TimeSpan.FromSeconds(2),
                scheduler);

            heartbeats
                .Timestamp(scheduler)
                .TimeInterval(scheduler)
                .Dump("Hearbeat Stream");

            var count = heartbeats
                .LostContact(scheduler)
                .Timestamp(scheduler)
                .TimeInterval(scheduler)
                .Dump("Lost Contact Stream")
                .Count();

            scheduler.Start();

            var countOfLostcontacts = count.Wait();

            Assert.AreEqual(3, countOfLostcontacts);

            Console.WriteLine("Here");
            Console.ReadKey();
        }       
    }

    class Event
    {

    }

    class Heartbeat : Event
    {         
    }

    class HeartbeatLost : Event
    {     
    }

    static class ServerUptime
    {
        public static IObservable<Event> LostContact(this IObservable<Event> events, IScheduler scheduler)
        {
            //Returns a HeartbeatLost if we don't see a heartbeat for 3 seconds.
            //return events
            //    .Where(t => t is Heartbeat)
            //    .Timeout(TimeSpan.FromSeconds(3), Observable.Return(new HeartbeatLost()), scheduler)
            //    .OfType<HeartbeatLost>();

            return events
                .Where(e => e is Heartbeat)
                .Buffer(TimeSpan.FromSeconds(2), scheduler)
                .Where(b => b.Count == 0)
                .SelectMany(b => Observable.Return(new HeartbeatLost()));
        }
    }

    static class Helpers
    {
        public static IObservable<T> Dump<T>(this IObservable<T> o, String name)
        {
            o.Subscribe(t => Console.WriteLine("{0} --> {1}", name, t), ex => Console.WriteLine("{0} --> {1}", name, ex), () => Console.WriteLine("{0} --> Completed", name));
            return o;
        }
    }

}
