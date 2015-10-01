using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportingCodeForTalk
{
    class Program
    {
        static void Main(string[] args)
        {            
            var heartbeats = Observable.Generate(
                0, 
                i => i < 5,
                i => i + 1, 
                _ => new Heartbeat(),
                i => i == 3 ? TimeSpan.FromSeconds(5) : TimeSpan.FromSeconds(2));

            heartbeats.Timestamp().TimeInterval().Dump("Hearbeat Stream");

            heartbeats.LostContact().Dump("Lost Contact Stream");

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
        public static IObservable<HeartbeatLost> LostContact(this IObservable<Event> heartbeatMessages)
        {
            //Returns a HeartbeatLost if we don't see a heartbeat for 3 seconds.
            return heartbeatMessages
                .OfType<Heartbeat>()
                .Cast<Event>()
                .Timeout(TimeSpan.FromSeconds(3), Observable.Return(new HeartbeatLost())).OfType<HeartbeatLost>();
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
