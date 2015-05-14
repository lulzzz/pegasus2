

//namespace Piraeus.ServiceModel.Protocols.Mqtt
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Globalization;
//    using System.Text;

//    public class SubscriptionTopicCollection : IList<Tuple<string, QualityOfServiceLevelType>>
//    {
//        public SubscriptionTopicCollection()
//        {
//            this.items = new List<Tuple<string, QualityOfServiceLevelType>>();
//        }

//        public SubscriptionTopicCollection(IEnumerable<Tuple<string, QualityOfServiceLevelType>> topics)
//        {
//            this.items = new List<Tuple<string, QualityOfServiceLevelType>>(topics);
//        }

//        private List<Tuple<string, QualityOfServiceLevelType>> items;

//        public int IndexOf(Tuple<string, QualityOfServiceLevelType> item)
//        {
//            return items.IndexOf(item);
//        }

//        public void Insert(int index, Tuple<string, QualityOfServiceLevelType> item)
//        {
//            items.Insert(index, item);
//        }

//        public void RemoveAt(int index)
//        {
//            items.RemoveAt(index);
//        }

//        public Tuple<string, QualityOfServiceLevelType> this[int index]
//        {
//            get { return items[index]; }
//            set { items[index] = value; }
//        }

//        public void Add(Tuple<string, QualityOfServiceLevelType> item)
//        {
//            items.Add(item);
//        }

//        public void Clear()
//        {
//            items.Clear();
//        }

//        public bool Contains(Tuple<string, QualityOfServiceLevelType> item)
//        {
//            return items.Contains(item);
//        }

//        public void CopyTo(Tuple<string, QualityOfServiceLevelType>[] array, int arrayIndex)
//        {
//            items.CopyTo(array, arrayIndex);
//        }

//        public int Count
//        {
//            get { return items.Count; }
//        }

//        public bool IsReadOnly
//        {
//            get { return false; }
//        }

//        public bool Remove(Tuple<string, QualityOfServiceLevelType> item)
//        {
//            return items.Remove(item);
//        }

//        public IEnumerator<Tuple<string, QualityOfServiceLevelType>> GetEnumerator()
//        {
//            return items.GetEnumerator();
//        }

//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }

//        public string CanonicalizeTopics()
//        {
//            List<string> topics = new List<string>();
//            foreach(Tuple<string, QualityOfServiceLevelType> item in this.items)
//            {
//                topics.Add(item.Item1.ToLower(CultureInfo.InvariantCulture));
//            }

//            topics.Sort();

//            StringBuilder builder = new StringBuilder();
//            foreach(string topic in topics)
//            {
//                builder.Append(topic);
//            }

//            return builder.ToString();
//        }

//        public QualityOfServiceLevelType? GetQualityOfServiceLevel(string topic)
//        {
//            foreach(Tuple<string, QualityOfServiceLevelType> item in items)
//            {
//                if(item.Item1.ToLower(CultureInfo.InvariantCulture) == topic.ToLower(CultureInfo.InvariantCulture))
//                {
//                    return item.Item2;
//                }
//            }

//            return null;
//        }

//        public IEnumerable<QualityOfServiceLevelType> GetQualityOfServiceLevels()
//        {
//            List<QualityOfServiceLevelType> list = new List<QualityOfServiceLevelType>();
//            foreach (Tuple<string, QualityOfServiceLevelType> item in items)
//            {
//                list.Add(item.Item2);
//            }

//            return list;
//        }
//    }
//}
