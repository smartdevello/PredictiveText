Imports System.Reflection.Emit
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Threading
Imports System.Windows
Imports System.Windows.Forms.VisualStyles
Imports InputHelper.EventArgs
Imports InputHelperLib
Imports Microsoft.Win32
Imports SharpHook

Public Class MainForm

    Public Sub New()
        InitializeComponent()
        'timer1.Start()  ' Processing events from Hooks involves message queue complexities.


        If MouseHook Is Nothing Then
            MouseHook = New InputHelper.Hooks.MouseHook
        End If
        AddHandler MouseHook.MouseDown, AddressOf MouseHook_MouseDown
        AddHandler MouseHook.MouseWheel, AddressOf MouseHook_MouseDown
        AddHandler KeyboardHook.KeyDown, AddressOf KeyboardHook_KeyDown
        AddHandler KeyboardHook.KeyUp, AddressOf KeyboardHook_KeyUp

        'Dim dpi1 = Registry.GetValue("HKEY_CURRENT_USER\\Control Panel\\Desktop", "LogPixels", 96)
        'Dim dpi3 = Registry.GetValue("HKEY_CURRENT_USER\\Control Panel\\Desktop\\WindowMetrics", "AppliedDPI", 0)

        If (Environment.OSVersion.Version.Major > 6) Then
            SetProcessDPIAware()
        End If
        Try
            Dim dpi = Registry.GetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ThemeManager", "LastLoadedDPI", "96")
            screenScale = 96 / CSng(dpi)
        Catch ex As Exception
            screenScale = 1
        End Try

    End Sub
    Protected Overrides Sub SetVisibleCore(ByVal value As Boolean)
        If Not Me.IsHandleCreated Then
            Me.CreateHandle()
            value = False
        End If
        MyBase.SetVisibleCore(value)
    End Sub


#Region "Test"

#End Region


#Region "Data Members & Structures "

    Dim MouseHook As New InputHelper.Hooks.MouseHook
    Dim KeyboardHook As New InputHelper.Hooks.KeyboardHook



    <StructLayout(LayoutKind.Sequential)>    ' Required by user32.dll
    Public Structure RECT
        Public Left As UInteger
        Public Top As UInteger
        Public Right As UInteger
        Public Bottom As UInteger
    End Structure

    Public Structure CopyDataStruct
        Public dwData As IntPtr
        Public cbData As Integer
        <MarshalAs(UnmanagedType.LPStr)>
        Public lpData As String
    End Structure

    <StructLayout(LayoutKind.Sequential)>    ' Required by user32.dll
    Public Structure GUITHREADINFO
        Public cbSize As UInteger
        Public flags As UInteger
        Public hwndActive As IntPtr
        Public hwndFocus As IntPtr
        Public hwndCapture As IntPtr
        Public hwndMenuOwner As IntPtr
        Public hwndMoveSize As IntPtr
        Public hwndCaret As IntPtr
        Public rcCaret As RECT
    End Structure

    Private Enum ShowWindowEnum
        Hide = 0
        ShowNormal = 1
        ShowMinimized = 2
        ShowMaximized = 3
        Maximize = 3
        ShowNormalNoActivate = 4
        Show = 5
        Minimize = 6
        ShowMinNoActivate = 7
        ShowNoActivate = 8
        Restore = 9
        ShowDefault = 10
        ForceMinimized = 11
    End Enum

    Private startPosition As Point = New Point(500, 500)       ' Point required for ToolTip movement by Mouse
    Private guiInfo As GUITHREADINFO                     ' To store GUI Thread Information
    Private caretPosition As Point                     ' To store Caret Position  
    Private targetAppHandle As IntPtr
    Private targetAppName As String = ""
    Private currentWord As String = ""                      'store the current word in typing
    Private prevTime As DateTime
    Private screenScale As Single = 1


#End Region

#Region "DllImports "

    ' - Retrieves Title Information of the specified window -
    <DllImport("user32.dll")>
    Private Shared Function GetWindowText(ByVal hWnd As Integer, ByVal text As StringBuilder, ByVal count As Integer) As Integer
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function MapVirtualKey(ByVal uCode As UInt32, ByVal uMapType As UInt32) As UInt32
    End Function

    ' - Retrieves Id of the thread that created the specified window -
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetWindowThreadProcessId(ByVal hWnd As Integer, <Out> ByRef lpdwProcessId As UInteger) As UInteger
    End Function

    ' - Retrieves information about active window or any specific GUI thread -
    <DllImport("user32.dll", EntryPoint:="GetGUIThreadInfo")>
    Public Shared Function GetGUIThreadInfo(ByVal tId As UInteger, <Out> ByRef threadInfo As GUITHREADINFO) As Boolean
    End Function

    ' - Retrieves Handle to the ForeGroundWindow -
    <DllImport("user32.dll")>
    Public Shared Function GetForegroundWindow() As IntPtr
    End Function

    ' - Converts window specific point to screen specific -
    <DllImport("user32.dll")>
    Public Shared Function ClientToScreen(ByVal hWnd As IntPtr, <Out> ByRef position As Point) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function ShowWindow(ByVal hWnd As IntPtr, ByVal flags As ShowWindowEnum) As Boolean
    End Function
    <DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(ByVal hwnd As IntPtr) As Integer

    End Function
    <DllImport("user32.dll")>
    Public Shared Function AttachThreadInput(ByVal idAttach As System.UInt32, ByVal idAttachTo As System.UInt32, ByVal fAttach As Boolean) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function BringWindowToTop(ByVal hwnd As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function SetProcessDPIAware() As Boolean
    End Function

    <DllImport("User32.dll", EntryPoint:="FindWindow")>
    Private Shared Function FindWindow(ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
    End Function
    <DllImport("User32.dll", EntryPoint:="SendMessage")>
    Private Shared Function SendMessage(ByVal hWnd As Integer, ByVal Msg As Integer, ByVal wParam As Integer, ByRef lParam As CopyDataStruct) As Integer
    End Function

    Private Declare Auto Function FindWindowEx Lib "user32.dll" (
ByVal hwndParent As IntPtr,
ByVal hwndChildAfter As IntPtr,
ByVal lpszClass As String,
ByVal lpszWindow As String
) As IntPtr

    Const WM_SETTEXT As Integer = &HC
    Const WM_KEYDOWN = &H100
    Const WM_KEYUP = &H101
    Const WM_SYSKEYDOWN = &H104
    Const WM_SYSKEYUP = &H105
    Const WM_COPYDATA As Integer = &H4A

#End Region

#Region "Event Handlers "
    Private Sub KeyboardHook_KeyDown(sender As Object, e As InputHelper.EventArgs.KeyboardHookEventArgs)
        'Console.WriteLine(KeyCodeToChar.GetKeyString(e.KeyCode, e.Modifiers))

        Dim hWnd As IntPtr = GetForegroundWindow()

        ' If Tooltip window is active window (Suppose user clicks on the Tooltip Window)
        If hWnd = Handle Then
            'then process nothing 
        Else
            'The forcus is on other app( target app)
            targetAppHandle = hWnd
        End If


        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Space Then
            If currentWord <> "" Then  ' We assume a word is completed, so we will save it on our db.
                DBManager.AddorUpdateWord(currentWord.ToLower(), targetAppName)
            End If
            Visible = False
            currentWord = ""
            suggestionWord.StringToFind = ""
            suggestionWord.words = New List(Of String)
            suggestionWord.SelectedIndex = -1

            Return
        End If


        If Not Char.IsControl(ChrW(MapVirtualKey(e.KeyCode, 2))) Then

            Dim c As Char = Chr(e.KeyCode)

            If c >= "A"c AndAlso c <= "Z"c Then

                currentWord = currentWord + c
                suggestionWord.StringToFind = currentWord.ToLower()
                UpdateSuggestionWordList(300)
                Console.WriteLine("current word is {0}", currentWord)
            Else
                currentWord = ""
                suggestionWord.StringToFind = currentWord.ToLower()
            End If



        Else
            If (e.KeyCode = Keys.Down Or e.KeyCode = Keys.Up) And Visible Then
                ForceForegroundWindow(Handle)
            End If

            'In the case the key was pressed on targetApp
            If hWnd <> Handle Then
                If suggestionWord.SelectedIndex >= 0 And (e.KeyCode = Keys.Tab Or e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Right) Then
                    Console.WriteLine("Key {0} was pressed outside Predicto", e.KeyCode)
                    e.Block = True
                    Dim selecteItem = suggestionWord.words(suggestionWord.SelectedIndex)
                    If selecteItem IsNot Nothing Then
                        Dim selectedString = selecteItem.ToString()
                        If currentWord.Length < selectedString.Length Then
                            Dim stringToSend As String = selectedString.Substring(currentWord.Length)
                            'My.Computer.Keyboard.SendKeys(stringToSend, True)
                            SendKeys.Send(stringToSend)
                            currentWord = ""
                        End If
                    End If
                End If
                If e.KeyCode = Keys.Escape Then
                    Visible = False

                End If
            End If

            Return
        End If

        ' If window explorer is active window (eg. user has opened any drive)
        ' Or for any failure when activeProcess is nothing
        ' 
        ' Get Current active Process

        Dim activeProcess As String = GetActiveProcess()
        If (activeProcess.ToLower().Contains("explorer") Or Equals(activeProcess, String.Empty)) Then
            ' Dissappear Tooltip
            Visible = False
        Else
            ' Otherwise Calculate Caret position
            EvaluateCaretPosition()
            If caretPosition.X = 0 And caretPosition.Y = 0 Then
                Visible = False
            Else
                ' Adjust ToolTip according to the Caret
                AdjustUI()

                ' Display current active Process on Tooltip
                'lblCurrentApp.Text = " You are Currently inside : " & activeProcess
                Visible = True
                targetAppName = activeProcess
            End If

        End If
    End Sub
    Private Sub KeyboardHook_KeyUp(sender As Object, e As InputHelper.EventArgs.KeyboardHookEventArgs)
        Dim hWnd As IntPtr = GetForegroundWindow()

        ' If Tooltip window is active window (Suppose user clicks on the Tooltip Window)
        If hWnd = Handle Then
            Console.WriteLine("Key {0} was pressed in Predicto", e.KeyCode)

            If suggestionWord.SelectedIndex >= 0 And (e.KeyCode = Keys.Tab Or e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Right) Then
                ForceForegroundWindow(targetAppHandle)
                Dim selectedString = suggestionWord.words(suggestionWord.SelectedIndex)
                If selectedString <> "" Then
                    If currentWord.Length < selectedString.Length Then
                        Dim stringToSend As String = selectedString.Substring(currentWord.Length)

                        SendKeys.Send(stringToSend)
                        'My.Computer.Keyboard.SendKeys(stringToSend, True)
                        DBManager.AddorUpdateWord(selectedString, targetAppName)
                        Console.WriteLine("Updated {0} to db", selectedString.ToLower())
                    End If

                End If

            End If
            If e.KeyCode = Keys.Down Or e.KeyCode = Keys.Up Then
                If suggestionWord.words.Count = 0 Then Return
                suggestionWord.Select()
                If e.KeyCode = Keys.Down Then
                    suggestionWord.SelectedIndex = suggestionWord.SelectedIndex + 1
                    If suggestionWord.SelectedIndex >= suggestionWord.words.Count Then suggestionWord.SelectedIndex = 0
                End If
                If e.KeyCode = Keys.Up Then
                    suggestionWord.SelectedIndex = suggestionWord.SelectedIndex - 1
                    If suggestionWord.SelectedIndex < 0 Then suggestionWord.SelectedIndex = suggestionWord.words.Count - 1
                End If
            End If
        Else
            'The forcus is on target app (for example notepad)
            targetAppHandle = hWnd
            If e.KeyCode = Keys.Back Then
                If currentWord.Length >= 1 Then
                    currentWord = currentWord.Substring(0, currentWord.Length - 1)
                    suggestionWord.StringToFind = currentWord.ToLower()
                    UpdateSuggestionWordList(300)
                End If
            End If

        End If
    End Sub
    Private Sub MouseHook_MouseDown(sender As Object, e As InputHelper.EventArgs.MouseHookEventArgs)
        If GetForegroundWindow() = Handle Then
            ' then do no processing
            Return
        End If
        Visible = False


    End Sub

    'Private Sub suggestionList_MouseEnter(sender As Object, e As EventArgs)
    '    ' Set the Mouse Cursor
    '    Cursor = Cursors.SizeAll
    'End Sub

    'Private Sub suggestionList_MouseMove(sender As Object, e As MouseEventArgs)
    '    ' If Left Button was pressed
    '    If e.Button = MouseButtons.Left Then
    '        ' then move the Tooltip
    '        Left += e.Location.X - startPosition.X
    '        Top += e.Location.Y - startPosition.Y
    '    End If
    'End Sub

    'Private Sub suggestionList_MouseDown(sender As Object, e As MouseEventArgs)
    '    ' Store start position of mouse when clicked down.
    '    ' It will be used to calculate offset during movement.
    '    startPosition = e.Location
    'End Sub


#End Region

#Region "Methods "

    ''' <summary>
    ''' Update Suggestion List words with the found from db, delays 300ms,
    '''  so it does not retrieve db for every keystroke
    ''' </summary>

    Private Sub UpdateSuggestionWordList(delay As Integer)
        Dim tmpWord = currentWord
        Dim delayTask As Task = Task.Delay(delay) ' delay milliseconds
        delayTask.ContinueWith(Sub(t)

                                   If tmpWord = currentWord Then  'We assume user ended typing a word, so start to check on db now
                                       Dim searched = DBManager.SearchWord(currentWord.ToLower())
                                       Console.WriteLine("found {0}", searched.Count)
                                       Me.Invoke(Sub()
                                                     Dim searchedWords As List(Of String) = New List(Of String)()

                                                     For Each item In searched
                                                         searchedWords.Add(item.Content)
                                                     Next
                                                     If searched.Count > 0 Then
                                                         suggestionWord.SelectedIndex = 0
                                                         Me.Height = searched.Count * suggestionWord.LineHeight + 10
                                                         suggestionWord.Height = searched.Count * suggestionWord.LineHeight
                                                         Me.Show()
                                                     Else
                                                         suggestionWord.SelectedIndex = -1
                                                         Me.Hide()
                                                     End If


                                                     suggestionWord.words = searchedWords
                                                     suggestionWord.StringToFind = currentWord.ToLower()



                                                 End Sub)

                                   End If
                               End Sub)
    End Sub
    ''' <summary>
    ''' This will bring a window to top of the screen, so it has focus, can get key events.
    ''' 
    ''' </summary>
    Private Sub ForceForegroundWindow(hWnd As IntPtr)

        Dim foreThread As UInteger = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero)
        Dim appThread As UInteger = GetWindowThreadProcessId(hWnd, IntPtr.Zero)
        If foreThread <> appThread Then
            AttachThreadInput(foreThread, appThread, True)
            BringWindowToTop(hWnd)
            ShowWindow(hWnd, ShowWindowEnum.Show)
            AttachThreadInput(foreThread, appThread, False)
        Else
            BringWindowToTop(hWnd)
            ShowWindow(hWnd, ShowWindowEnum.Show)
        End If
    End Sub


    ''' <summary>
    ''' This function will adjust Tooltip position and
    ''' will keep it always inside the screen area.
    ''' </summary>
    Private Sub AdjustUI()
        ' Get Current Screen Resolution
        Dim workingArea = SystemInformation.WorkingArea

        ' If current caret position throws Tooltip outside of screen area
        ' then do some UI adjustment.
        If caretPosition.X + Width > workingArea.Width Then
            caretPosition.X = caretPosition.X + 25
        End If

        If caretPosition.Y + Height > workingArea.Height Then
            caretPosition.Y = caretPosition.Y - Height - 50
        End If
        Console.WriteLine("Caret Position {0} {1}", caretPosition.X, caretPosition.Y)
        Left = caretPosition.X
        Top = caretPosition.Y
    End Sub

    ''' <summary>
    ''' Evaluates Cursor Position with respect to client screen.
    ''' </summary>
    Private Sub EvaluateCaretPosition()
        caretPosition = New Point()

        ' Fetch GUITHREADINFO
        GetCaretPosition()

        caretPosition.X = CInt(guiInfo.rcCaret.Left * screenScale) + 10
        caretPosition.Y = CInt(guiInfo.rcCaret.Bottom * screenScale)
        ClientToScreen(guiInfo.hwndCaret, caretPosition)

        'txtCaretX.Text = caretPosition.X.ToString()
        'txtCaretY.Text = caretPosition.Y.ToString()

    End Sub

    ''' <summary>
    ''' Get the caret position
    ''' </summary>
    Public Sub GetCaretPosition()
        guiInfo = New GUITHREADINFO()
        guiInfo.cbSize = CUInt(Marshal.SizeOf(guiInfo))

        ' Get GuiThreadInfo into guiInfo
        GetGUIThreadInfo(0, guiInfo)
    End Sub

    ''' <summary>
    ''' Retrieves name of active Process.
    ''' </summary>
    ''' <returns>Active Process Name</returns>
    Private Function GetActiveProcess() As String
        Const nChars = 256
        Dim handle = 0
        Dim Buff As StringBuilder = New StringBuilder(nChars)
        handle = CInt(GetForegroundWindow())

        ' If Active window has some title info
        If GetWindowText(handle, Buff, nChars) > 0 Then
            Dim lpdwProcessId As UInteger
            Dim dwCaretID As UInteger = GetWindowThreadProcessId(handle, lpdwProcessId)
            Dim dwCurrentID As UInteger = Thread.CurrentThread.ManagedThreadId
            Return Process.GetProcessById(lpdwProcessId).ProcessName
        End If
        ' Otherwise either error or non client region
        Return String.Empty
    End Function

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        'Me.ActiveControl = Nothing
        'Visible = False
        suggestionWord.StringToFind = ""
    End Sub


    Private Sub MainForm_KeyPress(sender As Object, e As KeyPressEventArgs) Handles MyBase.KeyPress
        Console.WriteLine("Key Pressed on Form {0}", e.KeyChar)
    End Sub

    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Console.WriteLine("Key Down on form {0}", e.KeyCode)
    End Sub




#End Region

End Class
