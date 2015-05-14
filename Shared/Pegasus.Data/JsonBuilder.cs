using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus2.Data
{
    public class JsonBuilder
    {
        public JsonBuilder()
        {
            builder = new StringBuilder();
        }

        private StringBuilder builder;

        public override string ToString()
        {
            EndJsonBuild();
            return builder.ToString();
        }

        private void StartJsonBuild()
        {
            builder.Append("{");
        }

        private void EndJsonBuild()
        {
            builder.Append("}");
        }

        public void BuildJsonField(string name, string part)
        {
            if(builder.ToString().Length == 0)
            {
                StartJsonBuild();
            }

            if (builder.Length != 1)
            {
                builder.Append(",");
            }

            builder.Append("\"");
            builder.Append(name);
            builder.Append("\"");
            builder.Append(": ");
            builder.Append("\"");
            builder.Append(part);
            builder.Append("\"");
        }

        public void BuildJsonFieldBool(string name, string part)
        {
            if (builder.ToString().Length == 0)
            {
                StartJsonBuild();
            }

            if (builder.Length != 1)
            {
                builder.Append(",");
            }

            builder.Append("\"");
            builder.Append(name);
            builder.Append("\"");
            builder.Append(": ");
            builder.Append("\"");
            if (part == "0")
            {
                builder.Append("false");
            }
            else if (part == "1")
            {
                builder.Append("true");
            }
            else
            {
                builder.Append(part);
            }

            builder.Append("\"");
        }

        public void BuildJsonComplexType(string name, string[] fieldNames, string[] fieldParts)
        {
            if (builder.ToString().Length == 0)
            {
                StartJsonBuild();
            }

            if (builder.Length != 1)
            {
                builder.Append(",");
            }

            builder.Append("\"");
            builder.Append(name);
            builder.Append("\"");
            builder.Append(": ");
            builder.Append("{");

            int index = 0;

            while (index < fieldNames.Length)
            {
                builder.Append("\"");
                builder.Append(fieldNames[index]);
                builder.Append("\"");
                builder.Append(": ");
                builder.Append("\"");
                builder.Append(fieldParts[index]);
                builder.Append("\"");

                if (index + 1 < fieldNames.Length)
                {
                    builder.Append(",");
                }
                index++;
            }

            builder.Append("}");
        }
    }
}
