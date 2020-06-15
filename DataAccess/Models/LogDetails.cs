using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DataAccess.Models
{
    // Fluent Interface - method chaining.
    public class LogDetails
    {
        private string _logClass;
        private string _logMethod;
        private string _logUser;

        public LogDetails SetLogClass(string logClass)
        {
            _logClass = logClass;
            return this;
        }
        public LogDetails SetLogMethod(string logMethod)
        {
            _logMethod = logMethod;
            return this;
        }
        public LogDetails SetLogUser(string logUser)
        {
            _logUser = logUser;
            return this;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod() => new StackTrace().GetFrame(1).GetMethod().Name;

        public override string ToString() =>
            string.Format("LogClass:{0};LogMethod:{1};LogUser:{2}",
                string.IsNullOrEmpty(_logClass) ? "Not provided" : _logClass,
                string.IsNullOrEmpty(_logMethod) ? "Not provided" : _logMethod,
                string.IsNullOrEmpty(_logUser) ? "Not provided" : _logUser
                );
    }
}
