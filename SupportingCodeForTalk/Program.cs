using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;

namespace SupportingCodeForTalk
{
    class Program
    {
        static void Main(string[] args)
        {            
            TestScheduler scheduler = new TestScheduler();

            var heartbeats = Observable.Generate(
                0, 
                i => i < 10,
                i => i + 1, 
                _ => new Heartbeat(),
                i => i == 3 || i == 5 ? TimeSpan.FromSeconds(5) : TimeSpan.FromSeconds(2),
                scheduler);

            heartbeats.Timestamp(scheduler).TimeInterval(scheduler).Dump("Hearbeat Stream");

            heartbeats.LostContact(scheduler).Dump("Lost Contact Stream");

            scheduler.Start();

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
        public static IObservable<HeartbeatLost> LostContact(this IObservable<Event> heartbeatMessages, IScheduler scheduler)
        {
            //Returns a HeartbeatLost if we don't see a heartbeat for 3 seconds.
            return heartbeatMessages
                .OfType<Heartbeat>()
                .Cast<Event>()
                .Timeout(TimeSpan.FromSeconds(3), Observable.Return(new HeartbeatLost()), scheduler).OfType<HeartbeatLost>();
        }
    }

    static class Helpers
    {
        public static void Dump<T>(this IObservable<T> o, String name)
        {
            o.Subscribe(t => Console.WriteLine("{0} --> {1}", name, t), ex => Console.WriteLine("{0} --> {1}", name, ex), () => Console.WriteLine("{0} --> Completed", name));
        }
    }

}
