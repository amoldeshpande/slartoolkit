#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Assembly Infos
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2010-02-24 00:35:56 +0100 (Mi, 24 Feb 2010) $
//   Changed in:        $Revision: 48548 $
//   Project:           $URL: https://slartoolkit.svn.codeplex.com/svn/trunk/SLARToolKit/Source/SLARToolKitSample/Properties/AssemblyInfo.cs $
//   Id:                $Id: AssemblyInfo.cs 48548 2010-02-23 23:35:56Z unknown $
//
//
//   Copyright (c) 2009-2011 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System;
using System.Windows;

namespace SLARToolKit3DSample
{
   public partial class App
   {

      public App()
      {
         this.Startup += this.Application_Startup;
         this.Exit += this.Application_Exit;
         this.UnhandledException += this.Application_UnhandledException;

         InitializeComponent();
      }

      private void Application_Startup(object sender, StartupEventArgs e)
      {
         this.RootVisual = new MainPage();
      }

      private void Application_Exit(object sender, EventArgs e)
      {

      }

      private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
      {
         // If the app is running outside of the debugger then report the exception using
         // the browser's exception mechanism. On IE this will display it a yellow alert 
         // icon in the status bar and Firefox will display a script error.
         if (!System.Diagnostics.Debugger.IsAttached)
         {

            // NOTE: This will allow the application to continue running after an exception has been thrown
            // but not handled. 
            // For production applications this error handling should be replaced with something that will 
            // report the error to the website and stop the application.
            e.Handled = true;
            Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
         }
      }

      private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
      {
         try
         {
            string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
            errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

            System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
         }
         catch (Exception)
         {
         }
      }
   }
}
