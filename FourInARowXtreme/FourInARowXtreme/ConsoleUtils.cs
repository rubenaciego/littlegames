using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

// Class that has some console functionality

public class ConsoleUtils
{
	private const int MF_BYCOMMAND = 0x00000000;
    public const int SC_MAXIMIZE = 0xF030;
    public const int SC_SIZE = 0xF000;
    private const int STD_OUTPUT_HANDLE = -11;
    private const int TMPF_TRUETYPE = 4;
    private const int LF_FACESIZE = 32;
    private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
    private const uint ENABLE_QUICK_EDIT = 0x0040;
    private const int STD_INPUT_HANDLE = -10; // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
    const UInt32 SW_HIDE =         0;
    const UInt32 SW_SHOWNORMAL =       1;
    const UInt32 SW_NORMAL =       1;
    const UInt32 SW_SHOWMINIMIZED =    2;
    const UInt32 SW_SHOWMAXIMIZED =    3;
    const UInt32 SW_MAXIMIZE =     3;
    const UInt32 SW_SHOWNOACTIVATE =   4;
    const UInt32 SW_SHOW =         5;
    const UInt32 SW_MINIMIZE =     6;
    const UInt32 SW_SHOWMINNOACTIVE =  7;
    const UInt32 SW_SHOWNA =       8;
    const UInt32 SW_RESTORE =      9;
    
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
    
    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetCurrentConsoleFontEx(IntPtr consoleOutput, bool maximumWindow, ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int dwType);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern int SetConsoleFont(IntPtr hOut, uint dwFontNum);

    [DllImport("user32.dll")]
    public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT rc);
    
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int w, int h, bool repaint);
    
	[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
	public static extern int MessageBoxW(int hWnd, String text, String caption, uint type);
	
	[DllImport("kernel32.dll")]
	public static extern IntPtr GetConsoleWindow();
	
	[DllImport("user32.dll")]
	public static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);
	
	private struct RECT { public int left, top, right, bottom; }
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal unsafe struct CONSOLE_FONT_INFO_EX
    {
        internal uint cbSize;
        internal uint nFont;
        internal COORD dwFontSize;
        internal int FontFamily;
        internal int FontWeight;
        internal fixed char FaceName[LF_FACESIZE];
     }
     
    [StructLayout(LayoutKind.Sequential)]
    internal struct COORD
    {
    	internal short X;
        internal short Y;

        internal COORD(short x, short y)
        {
        	X = x;
            Y = y;
        }
    }
    
    private struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public System.Drawing.Point ptMinPosition;
        public System.Drawing.Point ptMaxPosition;
        public System.Drawing.Rectangle rcNormalPosition;
    }

    public static bool GetMinimized(IntPtr handle)
    {
        WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
        placement.length = Marshal.SizeOf(placement);
        GetWindowPlacement(handle, ref placement);
        return placement.flags == SW_SHOWMINIMIZED;
    }
	
	public static void Maximize()
	{
		Console.SetWindowSize(170, 44);
		Console.SetBufferSize(170, 44);
		ShowWindow(GetConsoleWindow(), 3); //SW_MAXIMIZE = 3
		ShowWindow(GetConsoleWindow(), ~3);
		ShowWindow(GetConsoleWindow(), 3);
	}
	
	public static void CenterConsole()
	{
        IntPtr hWin = GetConsoleWindow();
        RECT rc;
        GetWindowRect(hWin, out rc);
        Screen scr = Screen.FromPoint(new Point(rc.left, rc.top));
        int x = scr.WorkingArea.Left + (scr.WorkingArea.Width - (rc.right - rc.left)) / 2;
        int y = scr.WorkingArea.Top + (scr.WorkingArea.Height - (rc.bottom - rc.top)) / 2;
        MoveWindow(hWin, x, y, rc.right - rc.left, rc.bottom - rc.top, false);
    }
	public static void SetConsoleFont(string fontName, short size)
    {
    	unsafe
        {
        	IntPtr hnd = GetStdHandle(STD_OUTPUT_HANDLE);
            if (hnd != INVALID_HANDLE_VALUE)
            {
            	CONSOLE_FONT_INFO_EX info = new CONSOLE_FONT_INFO_EX();
                info.cbSize = (uint)Marshal.SizeOf(info);
                // Set console font
                CONSOLE_FONT_INFO_EX newInfo = new CONSOLE_FONT_INFO_EX();
                newInfo.cbSize = (uint)Marshal.SizeOf(newInfo);
                newInfo.FontFamily = TMPF_TRUETYPE;
                IntPtr ptr = new IntPtr(newInfo.FaceName);
                Marshal.Copy(fontName.ToCharArray(), 0, ptr, fontName.Length);

                // Get some settings from current font.
                newInfo.dwFontSize = new COORD((short)(size / 2), size);
                newInfo.FontWeight = info.FontWeight;
                SetCurrentConsoleFontEx(hnd, false, ref newInfo);
            }
    	}
	}
	
	internal static bool DisableQuickEdit()
	{
		IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

        // get current console mode
        uint consoleMode;
        if (!GetConsoleMode(consoleHandle, out consoleMode))
        {
           // ERROR: Unable to get console mode.
           return false;
        }

        // Clear the quick edit bit in the mode flags
        consoleMode &= ~ENABLE_QUICK_EDIT;
        consoleMode &= ~(uint)0x0020;
        consoleMode &= ~(uint)0x0001;
        // set the new mode
        if (!SetConsoleMode(consoleHandle, consoleMode))
        {
           // ERROR: Unable to set console mode
           return false;
        }

        return true;
   }
   
   public static void DisableMenus()
   {
   		IntPtr handle = ConsoleUtils.GetConsoleWindow();
        IntPtr sysMenu = GetSystemMenu(handle, false);

        if (handle != IntPtr.Zero)
        {
            DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
            DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
        }
   }
}
