//
//   Project:           JellyPage
//   Modul:             JellyPage
//
//   Description:       Extensions class.
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright (c) 2009-2010 Rene Schulte
//

using System.Windows.Browser;

namespace System
{
   /// <summary>
   /// Utils class.
   /// </summary>
   internal static partial class Extensions
   {
      #region Extensions

      public static void Log(this object obj)
      {
         HtmlWindow window = HtmlPage.Window;
         var isConsoleAvailable = (bool)window.Eval("typeof(console) != 'undefined' && typeof(console.log) != 'undefined'");
         if (isConsoleAvailable)
         {
            var console = (window.Eval("console.log") as ScriptObject);
            if (console != null)
            {
               console.InvokeSelf(obj);
            }
         }
      }

      #endregion
  }
}