using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Logging;
using System.Xml;
using System.Xml.Serialization;

namespace MyGameServer.Tool
{
    public class Helper
    {

        public static int Int(object o)
        {
            return Convert.ToInt32(o);
        }

        public static float Float(object o)
        {
            return (float)Math.Round(Convert.ToSingle(o), 2);
        }

        public static long Long(object o)
        {
            return Convert.ToInt64(o);
        }

        /// <summary>
        /// 产生随机数:不会重复的随机数:左闭右开
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Random(int min, int max)
        {
            Random ra = new Random();
            System.Threading.Thread.Sleep(500);
            return ra.Next(min, max);
        }

        public static string Uid(string uid)
        {
            int position = uid.LastIndexOf('_');
            return uid.Remove(0, position + 1);
        }

        public static long GetTime()
        {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// 二进制解析
        /// 返回数字nSrc的二进制位的第nIndex位
        /// </summary>
        /// <param name="nSrc"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public static int BinBitParse(int nSrc, int nIndex)
        {
            if (nIndex < 1 || nIndex > 32)
                return -1;
            int n = (int)Math.Pow(2, nIndex - 1);
            int a = n & nSrc;
            return a >> (nIndex - 1);
        }
        /// <summary>
        /// 整形解析
        /// 返回数字nSrc的第nIndex位的数字
        /// 如:  nSrc=1024 nIndex=1  ==> 4
        ///      nSrc=1024 nIndex=3  ==> 0
        /// </summary>
        /// <param name="nSrc"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public static int IntBitparse(int nSrc, int nIndex)
        {
            if (nIndex < 1)
                return -1;
            int a = nSrc % (int)Math.Pow(10, nIndex);
            return a / (int)Math.Pow(10, nIndex - 1);
        }
       
        //获取UTC时间
        public static long GetUTCTime()
        {
            long time_t;
            System.DateTime dt1 = new System.DateTime(1970, 1, 1, 0, 0, 0);
            System.TimeSpan ts = System.DateTime.UtcNow - dt1;
            time_t = ts.Ticks / 10000000;
            return time_t;
        }

        //获取UTC时间
        public static long GetUTCTimeJava()
        {
            long time_t;
            System.DateTime dt1 = new System.DateTime(1970, 1, 1, 0, 0, 0);
            System.TimeSpan ts = System.DateTime.UtcNow - dt1;
            time_t = ts.Ticks / 10000;
            return time_t;
        }

        //获取UTC时间 毫秒
        public static long GetUTCTimeMillisecond()
        {
            long time_t;
            System.DateTime dt1 = new System.DateTime(1970, 1, 1, 0, 0, 0);
            System.TimeSpan ts = System.DateTime.UtcNow - dt1;
            time_t = ts.Ticks / 10000;
            return time_t;
        }

        /// <summary>
        /// 将日期转为秒数(以0时区为准,参数为中国时区时间)
        /// </summary>
        /// <param name="dTime"></param>
        /// <returns>返回UTC秒数</returns>
        public static long DataToSeconds(System.DateTime dTime)
        {
            if (null == dTime)
                return 0;
            System.DateTime dt = new System.DateTime(1970, 1, 1, 8, 0, 0);
            System.TimeSpan dt1 = dTime - dt;
            return dt1.Ticks / 10000000;
        }
        /// <summary>
        /// 将日期转为毫秒(以0时区为准,参数为中国时区时间)
        /// </summary>
        /// <param name="dTime"></param>
        /// <returns>返回UTC秒数</returns>
        public static long DataToMillisecond(System.DateTime dTime)
        {
            if (null == dTime)
                return 0;
            System.DateTime dt = new System.DateTime(1970, 1, 1, 8, 0, 0);
            System.TimeSpan dt1 = dTime - dt;
            return dt1.Ticks / 10000;
        }
        /// <summary>
        /// 获取服务器的UTC时间
        /// </summary>
        /// <param name="serverT"></param>
        /// <returns></returns>
        public static DateTime GetServerUTCTime(long serverT)
        {
            System.DateTime dt = new System.DateTime(1970, 1, 1, 0, 0, 0);
            System.DateTime dt1 = new System.DateTime(serverT * 10000 + dt.Ticks);
            return dt1;
        }

        public static DateTime GetUTCTime(uint scendnum)
        {
            System.DateTime dt = new System.DateTime(1970, 1, 1, 0, 0, 0);
            System.DateTime dt1 = new System.DateTime((long)(scendnum) * 1000 * 10000 + dt.Ticks);
            return dt1;
        }

        /// <summary>
        /// 将字符串解析为int
        /// </summary>
        /// <param name="szValue"></param>
        /// <returns></returns>
        public static int IntParse(String szValue)
        {
            int nValue = 0;
            if (String.IsNullOrEmpty(szValue))
                return 0;
            if (!int.TryParse(szValue, out nValue))
            {
                return 0;
            }
            return nValue;
        }
        /// <summary>
        /// 将字符串解析为uint
        /// </summary>
        /// <param name="szValue"></param>
        /// <returns></returns>
        public static uint uintParse(String szValue)
        {
            uint nValue = 0;
            if (String.IsNullOrEmpty(szValue))
                return 0;
            if (!uint.TryParse(szValue, out nValue))
            {
                return 0;
            }
            return nValue;
        }
        //循环左移
        public static uint ROL(uint opr, int n)
        {
            return ((opr << n) | (opr >> (32 - n)));
        }
        //循环右移
        public static uint ROR(uint opr, int n)
        {
            return ((opr >> n) | (opr << (32 - n)));
        }

        public static uint GetNowTimeUInt32()
        {
            return (uint)((DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond);
        }
        public static int ComputeHashValue(string str)
        {
            int res = 0;
            for (int i = 0, j = 1; i < str.Length; ++i, ++j)
            {
                res += (int)str[i] * j;
            }
            return (res % 256);
        }

        // 日志
        private static readonly ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 服务器日志
        /// </summary>
        public static void Log(string _strLog)
        {
            if (log != null)
            {
                log.Debug(_strLog);
            }
        }



        /// <summary>
        /// 解析XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_bytes"></param>
        /// <returns></returns>
        public static T LoadXML<T>(string _path)
        {
            if (!File.Exists(_path))
            {
                Log("文件不存在"+_path);
                return default(T);
            }
            FileStream stream = new FileStream(_path,FileMode.Open,FileAccess.Read);
            if (stream == null)
            {
                Log("文件读取失败" + _path);
                return default(T);
            }

            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                T data = (T)xs.Deserialize(stream);
                stream.Close();
                return data;
            }
            catch(Exception e)
            {
                Log("解析XML失败:"+typeof(T).Name +"--:"+e);
                return default(T);
            }
        }

        /// <summary>
        /// 九宫格判断
        /// </summary>
        /// <returns></returns>
        public static bool IsSudoKu(Vector3 _a, Vector3 _b)
        {
            float fDis = Vector3.Distance(_a,_b);
            if (fDis < 23)
            {
                return true;
            }
            return false;
        }
    }

    public class Vector3
    {
        public Vector3(float _x,float _y,float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public Vector3(Vector3 _a)
        {
            x = _a.x;
            y = _a.y;
            z = _a.z;
        }
        public float x;
        public float y;
        public float z;

        /// <summary>
        /// 单位向量
        /// </summary>
        public Vector3 nomalize
        {
            get
            {
                return Vector3.Normalize(this);
            }
        }

        /// <summary>
        /// 模
        /// </summary>
        public float magnitude
        {
            get 
            {
                return Vector3.Magnitude(this);
            }
        }
        /// <summary>
        /// 求距离
        /// </summary>
        /// <param name="_a"></param>
        /// <param name="_b"></param>
        /// <returns></returns>
        public static float Distance(Vector3 _a, Vector3 _b)
        {
            float x1 = _a.x - _b.x;
            x1 = x1 * x1;

            float y1 = _a.y - _b.y;
            y1 = y1 * y1;

            float z1 = _a.z - _b.z;
            z1 = z1 * z1;

            float fRes = (float)Math.Sqrt(x1 + y1 + z1);
            return fRes;
        }

        public static string GetVector(Vector3 _a)
        {
            string strRes = _a.x.ToString() + "," + _a.y + "," + _a.z;
            return strRes;
        }

        /// <summary>
        /// 求模
        /// </summary>
        /// <param name="_a"></param>
        /// <returns></returns>
        private static float Magnitude(Vector3 _a)
        {
            float x1 = _a.x * _a.x;
            float y1 = _a.y * _a.y;
            float z1 = _a.z * _a.z;
            float fRes = (float)Math.Sqrt(x1 + y1 + z1);
            return fRes;
        }

        /// <summary>
        /// 求单位向量
        /// </summary>
        /// <param name="_a"></param>
        /// <returns></returns>
        private static Vector3 Normalize(Vector3 _a)
        {
            Vector3 v = new Vector3(_a);

            float dis = Vector3.Magnitude(v);

            if (dis.Equals(1))
            {
                return v;
            }
            else if (dis < 1e-5)
            {
                return new Vector3(0, 0, 0);
            }
            else
            {
                v = Vector3.Div(v,dis);
            }
            return v;
        }

        private static Vector3 Div(Vector3 _a, float d)
        {
            if (d < 1e-5)
            {
                return new Vector3(0,0,0);
            }
            _a.x = _a.x / d;
            _a.y = _a.y / d;
            _a.z = _a.z / d;
            return _a;
        }

        /// <summary>
        /// 重载+运算
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
        {
            Vector3 result = new Vector3(lhs);
            result.x += rhs.x;
            result.y += rhs.y;
            result.z += rhs.z;
            return result;
        }


        /// <summary>
        /// 重载 - 运算符
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
        {
            Vector3 result = new Vector3(lhs);
            result.x -= rhs.x;
            result.y -= rhs.y;
            result.z -= rhs.z;
            return result;
        }

        /// <summary>
        /// 重载 * 运算
        /// </summary>
        /// <param name="_a"></param>
        /// <param name="_f"></param>
        /// <returns></returns>
        public static Vector3 operator * (Vector3 _a,float _f)
        {
            _a.x = _a.x * _f;
            _a.y = _a.y * _f;
            _a.z = _a.z * _f;
            return _a;
        }

        public override string ToString()
        {
            string strRes = this.x.ToString() + "," + this.y + "," + this.z;
            return strRes;
        }
    }
}
