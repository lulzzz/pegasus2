using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAE.FieldGateway.Channels
{
    public class FileSystemWriter : IDisposable
    {
        private static FileSystemWriter instance;

        public static FileSystemWriter Create()
        {
            if(instance == null)
            {
                instance = new FileSystemWriter();
            }

            return instance;
        }
        public FileSystemWriter()
        {
            try
            {
                if (writer == null)
                {
                    writer = new StreamWriter(filename);
                }

            }
            catch(Exception ex)
            {

            }
        }

        private bool disposed = false; 
        private readonly string launchFileName = "launch.txt";
        private readonly string filename = "fieldgatewaylog.txt";
        private StreamWriter writer;

        private void CreateWriter()
        {
            if(writer == null)
            {
                writer = new StreamWriter(filename);
            }
        }

        public void WriteLaunch(double lat, double lon, double alt)
        {
            using(StreamWriter sw = new StreamWriter(launchFileName,true))
            {
                sw.WriteLine(String.Format("{0} {1} {2}", lat, lon, alt));
                sw.Flush();
                sw.Close();
            }
        }

        public string[] ReadLaunch()
        {
            if(!File.Exists(launchFileName))
            {
                return null;
            }

            using(StreamReader sr = new StreamReader(launchFileName))
            {
                string line = sr.ReadLine();
                sr.Close();
                if(string.IsNullOrEmpty(line))
                {
                    return null;
                }
                else
                {
                    return line.Split(new char[] { ' ' });
                }
            }
        }

        public void Write(string name, byte[] message)
        {
            Write(name, Encoding.UTF8.GetString(message));
        }

        public void Write(string name, string message)
        {
            try
            {
                writer.WriteLine(String.Format("{0}-{1}", name, message));
                writer.Flush();           
            }
            catch(Exception ex)
            {
                Trace.TraceError("Cannot write to log");
                Trace.TraceError(ex.Message);
            }
        }

        public async Task WriteAsync(string name, byte[] message)
        {
            CreateWriter();
            await WriteAsync(name, Encoding.UTF8.GetString(message));
        }

        public async Task WriteAsync(string name, string message)
        {
            try
            {
                CreateWriter();
                await writer.WriteLineAsync(String.Format("{0}-{1}", name, message));
                await writer.FlushAsync();
                writer.Close();
            }
            catch(Exception ex)
            {
                Trace.TraceWarning("File writer exception.");
                Trace.TraceError(ex.Message);
                
                if(writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        public async Task CloseWriterAsync()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            });

            await Task.WhenAll(task);            
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if(writer != null)
                    {
                        this.writer.Dispose();
                    }
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            disposed = true;

        }
    }
}
