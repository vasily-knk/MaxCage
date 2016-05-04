using System;
using System.Collections.Generic;
using System.Threading;

namespace NamedPipes
{
    class Program
    {
        static Dictionary<EntityId, EntityData> ents_ = new Dictionary<EntityId, EntityData>();
        static Dictionary<ModelId, Model> models_ = new Dictionary<ModelId, Model>();

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
            data.transform = msg.transform;
            Console.WriteLine("Updating entity transform: {0}", data.getText());
        }

        static void OnMsgAddModel(MsgAddModel msg)
        {
            models_.Add(msg.id, msg.model);
            Console.WriteLine("Adding model: {0}, {1} verts", msg.model.name, msg.model.verts.Length);
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
            dispatcher.Add<MsgAddModel>(3, OnMsgAddModel);
            dispatcher.Add<MsgNextFrame>(100, OnNextFrame);

            using (var server = new TcpServer(dispatcher))
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
