#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("IJIRMiAdFhk6lliW5x0REREVEBM0PuRxj3fUnND9iI4K6xO1nWtZTJfqUaKOR10vNEBk/tIGSyq1RPBcjAnLdtLll6C4/AhPt7Z5H4DrdO8IDT1027z3zWWRfHC158UlPj3po5IRHxAgkhEaEpIRERCjfRozSK/jWV7yianO27MSjeRulexI2Rnzv8fA6OLIJDgHtA+J+edxdG1UbxkA49nHLfB/KSNREjxnFXaej8rZM28q+Vf8mIBnJq+A3Xz4h7hdIlFxLZqB5Tl2Z6rhkNeagVedDvqGbUGWgTsmB9EwgY8qiVMF54dzqSnIJtSqrGeaKemIlWthwJ8om8Xq5tUeeTNu76Jd0r5AnBt2wovki6cUhV7UOYATf0mGi0c0vRITERAR");
        private static int[] order = new int[] { 0,12,7,3,10,12,9,12,8,11,11,11,12,13,14 };
        private static int key = 16;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
