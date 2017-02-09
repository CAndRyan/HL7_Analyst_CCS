/***************************************************************
* Copyright (C) 2011 Jeremy Reagan, All Rights Reserved.
* I may be reached via email at: jeremy.reagan@live.com
* 
* This program is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; under version 2
* of the License.
* 
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
****************************************************************/

using System;
using System.IO;
using System.Windows.Forms;
using HL7Lib.Base;

namespace HL7_Analyst
{
    /// <summary>
    /// Log Class: Used to log exceptions to disk
    /// </summary>
    class Log : ILogWriter
    {
        /// <summary>
        /// The exception that is being logged
        /// </summary>
        public Exception Error { get; set; }
        /// <summary>
        /// The internal Instance of this singleton
        /// </summary>
        private static volatile Log _Instance;
        /// <summary>
        /// Serves as the locking object
        /// </summary>
        private static object syncRoot = new object();
        /// <summary>
        /// Singleton-style construction/access parameter
        /// </summary>
        public static Log Instance {
            get {
                if (_Instance == null) {
                    lock (syncRoot) {
                        if (_Instance == null)
                            _Instance = new Log();
                    }
                }

                return _Instance;
            }
        }
        /// <summary>
        /// Private constructor to force constuction and access through the Instance property
        /// </summary>
        private Log() { }
        /// <summary>
        /// Logs the specified exception to a log file on disk and returns an Error Report Form containing the error message
        /// </summary>
        /// <param name="err">Exception being logged</param>
        /// <returns>Returns an Error Report Form</returns>
        public IErrorReport LogException(Exception err)
        {
            frmErrorReport fer = new frmErrorReport(err);
            if (Directory.Exists(Path.Combine(Application.StartupPath, "Logs")))
            {
                StreamWriter sw = new StreamWriter(Path.Combine(Path.Combine(Application.StartupPath, "Logs"), DateTime.Now.ToString("yyyyMMdd") + ".log"), true);
                sw.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));                
                sw.WriteLine(err.Message);                                
                sw.WriteLine(err.StackTrace);
                sw.WriteLine("");
                sw.Close();
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Logs"));
                LogException(err);
            }
            return fer;
        }
    }
}
