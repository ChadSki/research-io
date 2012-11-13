using System;
using System.Text;
using System.Diagnostics;

namespace io
{
    public class IoString : IoObject
    {
        public override string name { get { return "String"; } }
        public string value = String.Empty;

        public char[] asCharArray { get { return value.ToCharArray(); } }

		public new static IoString createProto(IoState state)
		{
			IoString s = new IoString();
			return s.proto(state) as IoString;
		}

        public new static IoString createObject(IoState state)
        {
            IoString s = new IoString();
            return s.clone(state) as IoString;
        }

        public static IoString createObject(IoString symbol)
        {
            IoString str = new IoString();
            str = str.clone(symbol.state) as IoString;
            str.value = symbol.value;
            return str;
        }

        public static IoString createObject(IoState state, string symbol)
        {
            IoString str = new IoString();
            str = str.clone(state) as IoString;
            str.value = symbol;
            return str;
        }

        public static IoString createSymbolInMachine(IoState state, string symbol)
        {
            if (state.symbols[symbol] == null)
                state.symbols[symbol] = IoString.createObject(state, symbol);
            return state.symbols[symbol] as IoString;
        }

        public override IoObject proto(IoState state)
		{
			IoString pro = new IoString();
            pro.state = state;
		//	pro.tag.cloneFunc = new IoTagCloneFunc(this.clone);
        //    pro.tag.compareFunc = new IoTagCompareFunc(this.compare);
            pro.createSlots();
            pro.createProtos();
            state.registerProtoWithFunc(name, new IoStateProto(name, pro, new IoStateProtoFunc(this.proto)));
			pro.protos.Add(state.protoWithInitFunc("Object"));

            IoCFunction[] methodTable = new IoCFunction[] {
                new IoCFunction("appendStr", new IoMethodFunc(IoString.slotAppendStr)),
                new IoCFunction("at", new IoMethodFunc(IoString.slotAt)),
                new IoCFunction("reverse", new IoMethodFunc(IoString.slotReverse)),
			};

			pro.addTaglessMethodTable(state, methodTable);
			return pro;
		}

        public static IoObject slotAppendStr(IoObject target, IoObject locals, IoObject message)
        {
            IoMessage m = message as IoMessage;
            IoString o = target as IoString;
            IoString arg = m.localsSymbolArgAt(locals, 0);
            o.value += arg.value.Replace(@"\""", "\"");
            return o;
        }

        public static IoObject slotAt(IoObject target, IoObject locals, IoObject message)
        {
            IoMessage m = message as IoMessage;
            IoString o = target as IoString;
            IoString res = IoString.createObject(target.state);
            IoNumber arg = m.localsNumberArgAt(locals, 0);
            res.value += o.value.Substring(arg.asInt(),1);
            return res;
        }

        public static IoObject slotReverse(IoObject target, IoObject locals, IoObject message)
        {
            IoMessage m = message as IoMessage;
            IoString o = target as IoString;
            IoString res = IoString.createObject(target.state);
            char[] A = o.asCharArray;
            Array.Reverse(A);
            res.value = new string(A);
            return res;
        }

		public override IoObject clone(IoState state)
		{
			IoString proto = state.protoWithInitFunc(name) as IoString;
			IoString result = new IoString();
			result.state = state;
            result.value = proto.value;
			result.createProtos();
			result.createSlots();
			result.protos.Add(proto);
			return result;
		}

        public override int compare(IoObject v)
        {
			if (v is IoString) return this.value.CompareTo((v as IoString).value);
            return base.compare(v);
        }

        public override void print()
        {
            Console.Write("{0}", value);
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static IoString rawAsUnquotedSymbol(IoString s)
        {
            string str = "";
            if (s.value.StartsWith("\"")) str = s.value.Substring(1, s.value.Length - 1);
            if (s.value.EndsWith("\"")) str = str.Substring(0,s.value.Length-2);
            return IoString.createObject(s.state, str);
        }

        public static IoString rawAsUnescapedSymbol(IoString s)
        {
            string str = "";
            int i = 0;
            while (i < s.value.Length)
            {
                char c = s.value[i];
                if (c != '\\')
                {
                    str += c;
                }
                else
                {
                    c = s.value[i];
                    switch (c)
                    {
                        case 'a': c = '\a'; break;
                        case 'b': c = '\b'; break;
                        case 'f': c = '\f'; break;
                        case 'n': c = '\n'; break;
                        case 'r': c = '\r'; break;
                        case 't': c = '\t'; break;
                        case 'v': c = '\v'; break;
                        case '\0': c = '\\'; break;
                        default:
                            if (c > '0' && c < '9')
                            {
                                c -= '0';
                            }
                            break;
                    }
                    str += c;
                }

                i++;
            }
            return IoString.createObject(s.state, str);
        }
    }
}
