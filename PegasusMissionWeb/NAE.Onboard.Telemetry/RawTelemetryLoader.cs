using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NAE.Onboard.Telemetry
{
    public class RawTelemetryLoader
    {
        public static List<EagleRawTelemetry> Load(List<TelemetryValues> list)
        {
            List<EagleRawTelemetry> lt = new List<EagleRawTelemetry>();


            for (int i=0; i < list.Count; i++)
            {
                string name = list[i].PropertyName;
                
                for(int j=0; j < list[i].Values.Count(); j++)
                {
                    if( i == 0)
                    {
                        EagleRawTelemetry t = new EagleRawTelemetry() { Timestamp = list[i].Timestamps[j] };
                        t.GetType().InvokeMember(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, Type.DefaultBinder, t, new object[] { list[i].Values[j] });
                        lt.Add(t);
                    }
                    else
                    {
                        EagleRawTelemetry t = lt[j];
                        //t.Timestamp = list[i].Timestamps[j];
                        t.GetType().InvokeMember(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, Type.DefaultBinder, t, new object[] { list[i].Values[j] });
                        lt[j] = t;
                    }
                }
            }

            return lt;
        }

        private EagleRawTelemetry telemetry;
    }
}
