using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7Lib.Base {
    /// <summary>
    /// IErrorReport interface: To provide a return object from logging exceptions
    /// </summary>
    public interface IErrorReport {
        void Report();
    }
    /// <summary>
    /// ILogWriter interface: To faciliate the use of error and warning logs
    /// </summary>
    public interface ILogWriter {
        IErrorReport LogException(Exception err);
    }
    /// <summary>
    /// LogWriter Class: A default log writer
    /// </summary>
    public sealed class LogWriter : ILogWriter {
        /// <summary>
        /// The exception that is being logged
        /// </summary>
        public Exception Error { get; set; }
        /// <summary>
        /// The internal Instance of this singleton
        /// </summary>
        private static volatile LogWriter _Instance;
        /// <summary>
        /// Serves as the locking object
        /// </summary>
        private static object syncRoot = new object();
        /// <summary>
        /// Singleton-style construction/access parameter
        /// </summary>
        public static LogWriter Instance {
            get {
                if (_Instance == null) {
                    lock (syncRoot) {
                        if (_Instance == null)
                            _Instance = new LogWriter();
                    }
                }

                return _Instance;
            }
        }
        /// <summary>
        /// Private constructor to force constuction and access through the Instance property
        /// </summary>
        private LogWriter() { }
        /// <summary>
        /// Logs the specified exception to a log file on disk and returns an Error Report containing the error message
        /// </summary>
        /// <param name="err">Exception being logged</param>
        /// <returns>Returns an Error Report</returns>
        public IErrorReport LogException(Exception err) {
            return new ErrorReport(err);
        }
    }
    /// <summary>
    /// ErrorReport Class: A default error report
    /// </summary>
    public class ErrorReport : IErrorReport {
        /// <summary>
        /// The exception that is being reported
        /// </summary>
        public Exception Error { get; set; }
        /// <summary>
        /// Initialization Method: Sets the text displays to the values of the error message.
        /// </summary>
        /// <param name="err">Error that was thrown.</param>
        public ErrorReport(Exception err) {
            Error = err;
        }
        /// <summary>
        /// Report the provided error
        /// </summary>
        public void Report() {
            return; //TODO: Implement the report method
        }
    }
}
