Imports System.Reflection.Emit
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Threading
Imports System.Windows
Imports System.Windows.Forms.VisualStyles
Imports InputHelper.EventArgs
Imports InputHelperLib

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


    'Private WithEvents kbHook As New KeyboardHook
    Dim MouseHook As New InputHelper.Hooks.MouseHook
    Dim KeyboardHook As New InputHelper.Hooks.KeyboardHook

    <StructLayout(LayoutKind.Sequential)>    ' Required by user32.dll
    Public Structure RECT
        Public Left As UInteger
        Public Top As UInteger
        Public Right As UInteger
        Public Bottom As UInteger
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


#End Region

#Region "Event Handlers "
    Private Sub KeyboardHook_KeyDown(sender As Object, e As KeyboardHookEventArgs)
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
            currentWord = ""    'reset current word , assuming new word would be begun.
            suggestionList.stringToFind = ""
            Return
        End If


        If Not Char.IsControl(ChrW(MapVirtualKey(e.KeyCode, 2))) Then

            Dim c As Char = Chr(e.KeyCode)

            If c >= "A"c AndAlso c <= "Z"c Then
                currentWord = currentWord + c
                suggestionList.stringToFind = currentWord.ToLower()
                UpdateSuggestionWordList(300)
                Console.WriteLine("current word is {0}", currentWord)
            Else
                currentWord = ""
                suggestionList.stringToFind = currentWord.ToLower()
            End If

        Else
            If (e.KeyCode = Keys.Down Or e.KeyCode = Keys.Up) And Visible Then
                ForceForegroundWindow(Handle)
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
            ' Otherwise Calculate Caret positiondfsefsfesfse
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
    Private Sub KeyboardHook_KeyUp(sender As Object, e As KeyboardHookEventArgs)
        Dim hWnd As IntPtr = GetForegroundWindow()
        ' If Tooltip window is active window (Suppose user clicks on the Tooltip Window)
        If hWnd = Handle Then
            If e.KeyCode = Keys.Tab Or e.KeyCode = Keys.Enter Then
                ForceForegroundWindow(targetAppHandle)
                Dim selectedString = suggestionList.SelectedItem.ToString()
                If selectedString <> "" Then
                    If currentWord.Length < selectedString.Length Then
                        Dim stringToSend As String = selectedString.Substring(currentWord.Length)
                        My.Computer.Keyboard.SendKeys(stringToSend, True)
                        DBManager.AddorUpdateWord(selectedString, targetAppName)
                        Console.WriteLine("Updated {0} to db", selectedString.ToLower())
                    End If

                End If

            End If
            Return
        Else
            'The forcus is on other app( target app)
            targetAppHandle = hWnd
            If e.KeyCode = Keys.Back Then
                If currentWord.Length >= 1 Then
                    currentWord = currentWord.Substring(0, currentWord.Length - 1)
                    suggestionList.stringToFind = currentWord.ToLower()

                    UpdateSuggestionWordList(300)
                End If
            End If
        End If
    End Sub
    Private Sub MouseHook_MouseDown(sender As Object, e As MouseHookEventArgs)
        If GetForegroundWindow() = Handle Then
            ' then do no processing
            Return
        End If
        Visible = False


    End Sub

    Private Sub suggestionList_MouseEnter(sender As Object, e As EventArgs) Handles suggestionList.MouseEnter
        ' Set the Mouse Cursor
        Cursor = Cursors.SizeAll
    End Sub

    Private Sub suggestionList_MouseMove(sender As Object, e As MouseEventArgs) Handles suggestionList.MouseMove
        ' If Left Button was pressed
        If e.Button = MouseButtons.Left Then
            ' then move the Tooltip
            Left += e.Location.X - startPosition.X
            Top += e.Location.Y - startPosition.Y
        End If
    End Sub

    Private Sub suggestionList_MouseDown(sender As Object, e As MouseEventArgs) Handles suggestionList.MouseDown
        ' Store start position of mouse when clicked down.
        ' It will be used to calculate offset during movement.
        startPosition = e.Location
    End Sub


#End Region

#Region "Methods "

    ' Update Suggestion List words with the found from db, delays 300ms, so it does not retrieve db for every keystroke
    Private Sub UpdateSuggestionWordList(delay As Integer)
        Dim tmpWord = currentWord
        Dim delayTask As Task = Task.Delay(delay) ' delay milliseconds
        delayTask.ContinueWith(Sub(t)

                                   If tmpWord = currentWord Then  'We assume user ended typing a word, so start to check on db now
                                       Dim searched = DBManager.SearchWord(currentWord.ToLower())
                                       Console.WriteLine("found {0}", searched.Count)
                                       Me.Invoke(Sub()
                                                     suggestionList.Items.Clear()
                                                     For Each item In searched
                                                         suggestionList.Items.Add(item.Content)
                                                     Next

                                                     suggestionList.Height = suggestionList.ItemHeight * searched.Count



                                                 End Sub)

                                   End If
                               End Sub)
    End Sub
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

        caretPosition.X = CInt(guiInfo.rcCaret.Left) + 10
        caretPosition.Y = CInt(guiInfo.rcCaret.Bottom)
        Console.WriteLine("caretPosition {0} {1}", caretPosition.X, caretPosition.Y)
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
        Me.ActiveControl = Nothing
        Visible = False
        suggestionList.stringToFind = ""
    End Sub

    'Private Sub predictList_KeyPress(sender As Object, e As KeyPressEventArgs) Handles predictList.KeyPress
    '    Console.WriteLine("Key Pressed on List", e.KeyChar)
    'End Sub

    Private Sub MainForm_KeyPress(sender As Object, e As KeyPressEventArgs) Handles MyBase.KeyPress
        Console.WriteLine("Key Pressed on Form", e.KeyChar)
    End Sub




#End Region

End Class
