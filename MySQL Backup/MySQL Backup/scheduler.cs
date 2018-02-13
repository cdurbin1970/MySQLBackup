using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Calendar;

namespace MySQL_Backup {
    class scheduler {
        public string Name {
                get { return GetType().Name; }
            }

            public void Run() {
               
                EventLog log = new EventLog("Application",".","MySQL Backup");

                // First we must get a reference to a scheduler
                NameValueCollection properties = new NameValueCollection();
                properties["quartz.scheduler.instanceName"] = "XmlConfiguredInstance";
                // set thread pool info
                properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
                properties["quartz.threadPool.threadCount"] = "5";
                properties["quartz.threadPool.threadPriority"] = "Normal";
                // job initialization plugin handles our xml reading, without it defaults are used
                properties["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz";
                properties["quartz.plugin.xml.fileNames"] = "configs/jobs.xml";
                ISchedulerFactory sf = new StdSchedulerFactory(properties);
                IScheduler sched = sf.GetScheduler();
                // we need to add calendars manually, lets create a silly sample calendar
                var dailyCalendar = new DailyCalendar("00:01", "23:59");
                dailyCalendar.InvertTimeRange = true;
                sched.AddCalendar("cal1", dailyCalendar, false, false);
                log.WriteEntry("MySQL Backup Job Scheduler Initialization Complete",EventLogEntryType.Information);
                // all jobs and triggers are now in scheduler
                
                // Start up the scheduler (nothing can actually run until the 
                // scheduler has been started)
                sched.Start();
                // log.Info("------- Started Scheduler -----------------");
                log.WriteEntry("MySQL Backup Started Scheduler", EventLogEntryType.Information);

            // wait long enough so that the scheduler as an opportunity to 
            // fire the triggers
            // log.Info("------- Waiting 30 seconds... -------------");

                //while ()

                try {
                    Thread.Sleep(30 * 1000);
                }
                catch (ThreadInterruptedException) {
                }

                // shut down the scheduler
                //log.Info("------- Shutting Down ---------------------");
                sched.Shutdown(true);
                //log.Info("------- Shutdown Complete -----------------");
            }
        }
    }