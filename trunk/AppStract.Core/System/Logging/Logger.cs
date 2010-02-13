﻿#region Copyright (C) 2008-2009 Simon Allaeys

/*
    Copyright (C) 2008-2009 Simon Allaeys
 
    This file is part of AppStract

    AppStract is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    AppStract is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with AppStract.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.IO;
using System.Text;
using System.Threading;

namespace AppStract.Core.System.Logging
{
  public abstract class Logger
  {

    #region Variables

    protected object _syncRoot;
    protected TextWriter _writer;
    protected LogLevel _level;
    protected LogType _logType;

    #endregion

    #region Properties

    public LogLevel LogLevel
    {
      get { return _level; }
      set { _level = value; }
    }

    public LogType Type
    {
      get { return _logType; }
    }

    #endregion

    #region Constructors

    protected Logger() { }

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <param name="logType"></param>
    /// <param name="logLevel"></param>
    /// <param name="writer"></param>
    protected Logger(LogType logType, LogLevel logLevel, TextWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      _level = logLevel;
      _logType = logType;
      _writer = writer;
      _syncRoot = new object();
    }

    #endregion

    #region Public Methods

    public virtual void Log(LogMessage logMessage)
    {
      if (logMessage.Level > _level)
        return;
      if (logMessage.Exception == null)
        Write(FormatLogMessage(logMessage));
      else
        Write(FormatLogMessage(logMessage));
    }

    public virtual void Warning(string format, params object[] args)
    {
      if (LogLevel.Warning <= _level)
        Write(FormatLogMessage(new LogMessage(LogLevel.Warning, string.Format(format, args))));
    }

    public virtual void Warning(string format, Exception exception, params object[] args)
    {
      if (LogLevel.Warning <= _level)
        Write(FormatLogMessage(new LogMessage(LogLevel.Warning, string.Format(format, args), exception)));
    }

    public virtual void Message(string format, params object[] args)
    {
      if (LogLevel.Information <= _level)
        Write(FormatLogMessage(new LogMessage(LogLevel.Information, string.Format(format, args))));
    }

    public virtual void Message(string format, Exception exception, params object[] args)
    {
      if (LogLevel.Information <= _level)
        Write(FormatLogMessage(new LogMessage(LogLevel.Information, string.Format(format, args), exception)));
    }

    public virtual void Error(string format, params object[] args)
    {
      if (LogLevel.Error <= _level)
        Write(FormatLogMessage(new LogMessage(LogLevel.Error, string.Format(format, args))));
    }

    public virtual void Error(string format, Exception exception, params object[] args)
    {
      if (LogLevel.Error <= _level)
        Write(FormatLogMessage(new LogMessage(LogLevel.Error, string.Format(format, args), exception)));
    }

    public virtual void Critical(string format, params object[] args)
    {
      if (LogLevel.Critical <= _level)
        Write(FormatLogMessage(new LogMessage(LogLevel.Critical, string.Format(format, args))));
    }

    public virtual void Critical(string format, Exception exception, params object[] args)
    {
      if (LogLevel.Critical <= _level)
        Write(FormatLogMessage(new LogMessage(LogLevel.Critical, string.Format(format, args), exception)));
    }

    public virtual void Debug(string format, params object[] args)
    {
      if (LogLevel.Debug <= _level)
        Write(FormatLogMessage(new LogMessage(LogLevel.Debug, string.Format(format, args))));
    }

    public virtual void Debug(string format, Exception exception, params object[] args)
    {
      if (LogLevel.Debug <= _level)
        Write(FormatLogMessage(new LogMessage(LogLevel.Debug, string.Format(format, args), exception)));
    }

    #endregion

    #region Protected Methods

    protected virtual string FormatLogMessage(LogMessage message)
    {
      string formattedMessage
        = string.Format("{0} [{1}] [{2}] {3}",
                        message.DateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
                        message.Level,
                        Thread.CurrentThread.Name,
                        message.Message)
          + (message.Exception != null ? "\r\n" + FormatException(message.Exception, message.Level) : "");
      return formattedMessage;
    }

    protected virtual void Write(string message)
    {
      Monitor.Enter(_syncRoot);
      try
      {
        _writer.WriteLine(message);
        _writer.Flush();
      }
      finally
      {
        Monitor.Exit(_syncRoot);
      }
    }

    protected static string FormatException(Exception ex, LogLevel logLevel)
    {
      var exceptionFormatter = new StringBuilder();
      exceptionFormatter.AppendLine("Exception: " + ex);
      exceptionFormatter.AppendLine("  Message: " + ex.Message);
      exceptionFormatter.AppendLine("  Site   : " + ex.TargetSite);
      exceptionFormatter.AppendLine("  Source : " + ex.Source);
      var innerException = ex.InnerException;
      while (innerException != null)
      {
        exceptionFormatter.AppendLine("Inner Exception:");
        exceptionFormatter.AppendLine("\t" + innerException);
        exceptionFormatter.AppendLine("\t Message: " + innerException.Message);
        innerException = innerException.InnerException;
      }
      if (logLevel == Logging.LogLevel.Debug)
      {
        exceptionFormatter.AppendLine("Stack Trace:");
        exceptionFormatter.AppendLine(ex.StackTrace);
      }
      return exceptionFormatter.ToString();
    }

    #endregion

  }
}
