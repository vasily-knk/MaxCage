using System;
using System.Collections.Generic;
using System.Threading;

namespace NamedPipes
{
    using EntityId = UInt32;

    class Program
    {
        static Dictionary<EntityId, EntityData> ents_ = new Dictionary<EntityId, EntityData>();

        static void OnMsgAddEntity(MsgAddEntity msg)
        {
            ents_.Add(msg.id, msg.data);
            Console.WriteLine("Adding entity: {0}", msg.data.getText());
        }

        static void OnMsgDeleteEntity(MsgDeleteEntity msg)
        {
            EntityData data = ents_[msg.id];
            Console.WriteLine("Deleting entity: {0}", data.getText());
            ents_.Remove(msg.id);
        }

        static void OnMsgUpdateEntityTransform(MsgUpdateEntityTransform msg)
        {
            EntityData data = ents_[msg.id];
            Console.WriteLine("Updating entity: {0}", data.getText());
        }

        static void OnNextFrame(MsgNextFrame msg)
        {

        }

        static void Main(string[] args)
        {
            var dispatcher = new Dispatcher();
            dispatcher.Add<MsgAddEntity>(0, OnMsgAddEntity);
            dispatcher.Add<MsgDeleteEntity>(1, OnMsgDeleteEntity);
            dispatcher.Add<MsgUpdateEntityTransform>(2, OnMsgUpdateEntityTransform);
            dispatcher.Add<MsgNextFrame>(3, OnNextFrame);

            using (var server = new Server("my_pipe", dispatcher))
            {
                server.Start();

                while(server.isWorking())
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
