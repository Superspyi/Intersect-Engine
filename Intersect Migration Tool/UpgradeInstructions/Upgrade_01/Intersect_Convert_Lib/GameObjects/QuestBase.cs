﻿using System.Collections.Generic;
using System.Linq;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_1.Intersect_Convert_Lib.GameObjects
{
    public class QuestBase : DatabaseObject
    {
        //General
        public new const string DATABASE_TABLE = "quests";

        public new const GameObject OBJECT_TYPE = GameObject.Quest;
        protected static Dictionary<int, DatabaseObject> sObjects = new Dictionary<int, DatabaseObject>();

        //Requirements
        public int ClassReq;

        public string EndDesc = "";
        public int ItemReq;
        public int LevelReq;

        public string Name = "New Quest";
        public int QuestReq;
        public string StartDesc = "";
        public int SwitchReq;

        //Tasks
        public List<QuestTask> Tasks = new List<QuestTask>();

        public int VariableReq;
        public int VariableValue;

        public QuestBase(int id) : base(id)
        {
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            StartDesc = myBuffer.ReadString();
            EndDesc = myBuffer.ReadString();
            ClassReq = myBuffer.ReadInteger();
            ItemReq = myBuffer.ReadInteger();
            LevelReq = myBuffer.ReadInteger();
            QuestReq = myBuffer.ReadInteger();
            SwitchReq = myBuffer.ReadInteger();
            VariableReq = myBuffer.ReadInteger();
            VariableValue = myBuffer.ReadInteger();

            var maxTasks = myBuffer.ReadInteger();
            Tasks.Clear();
            for (int i = 0; i < maxTasks; i++)
            {
                QuestTask q = new QuestTask
                {
                    Objective = myBuffer.ReadInteger(),
                    Desc = myBuffer.ReadString(),
                    Data1 = myBuffer.ReadInteger(),
                    Data2 = myBuffer.ReadInteger(),
                    Experience = myBuffer.ReadInteger()
                };
                for (int n = 0; n < Options.MaxNpcDrops; n++)
                {
                    q.Rewards[n].ItemNum = myBuffer.ReadInteger();
                    q.Rewards[n].Amount = myBuffer.ReadInteger();
                }
                Tasks.Add(q);
            }

            myBuffer.Dispose();
        }

        public byte[] QuestData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteString(StartDesc);
            myBuffer.WriteString(EndDesc);
            myBuffer.WriteInteger(ClassReq);
            myBuffer.WriteInteger(ItemReq);
            myBuffer.WriteInteger(LevelReq);
            myBuffer.WriteInteger(QuestReq);
            myBuffer.WriteInteger(SwitchReq);
            myBuffer.WriteInteger(VariableReq);
            myBuffer.WriteInteger(VariableValue);

            myBuffer.WriteInteger(Tasks.Count);
            for (int i = 0; i < Tasks.Count; i++)
            {
                myBuffer.WriteInteger(Tasks[i].Objective);
                myBuffer.WriteString(Tasks[i].Desc);
                myBuffer.WriteInteger(Tasks[i].Data1);
                myBuffer.WriteInteger(Tasks[i].Data2);
                myBuffer.WriteInteger(Tasks[i].Experience);
                for (int n = 0; n < Options.MaxNpcDrops; n++)
                {
                    myBuffer.WriteInteger(Tasks[i].Rewards[n].ItemNum);
                    myBuffer.WriteInteger(Tasks[i].Rewards[n].Amount);
                }
            }

            return myBuffer.ToArray();
        }

        public static QuestBase GetQuest(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return (QuestBase) sObjects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return ((QuestBase) sObjects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return QuestData();
        }

        public override string GetTable()
        {
            return DATABASE_TABLE;
        }

        public override GameObject GetGameObjectType()
        {
            return OBJECT_TYPE;
        }

        public static DatabaseObject Get(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return sObjects[index];
            }
            return null;
        }

        public override void Delete()
        {
            sObjects.Remove(GetId());
        }

        public static void ClearObjects()
        {
            sObjects.Clear();
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            sObjects.Remove(index);
            sObjects.Add(index, obj);
        }

        public static int ObjectCount()
        {
            return sObjects.Count;
        }

        public static Dictionary<int, QuestBase> GetObjects()
        {
            Dictionary<int, QuestBase> objects = sObjects.ToDictionary(k => k.Key, v => (QuestBase) v.Value);
            return objects;
        }

        public class QuestTask
        {
            public int Data1;
            public int Data2;
            public string Desc = "";
            public int Experience;
            public int Objective;
            public List<QuestReward> Rewards = new List<QuestReward>();

            public QuestTask()
            {
                for (int i = 0; i < Options.MaxNpcDrops; i++)
                {
                    Rewards.Add(new QuestReward());
                }
            }
        }

        public class QuestReward
        {
            public int Amount;
            public int ItemNum;
        }
    }
}