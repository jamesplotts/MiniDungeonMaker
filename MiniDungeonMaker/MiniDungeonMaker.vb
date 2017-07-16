Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO





Public Class MiniDungeonMaker

    Dim pvtTileset As New Tileset
    Dim PaletteKey As New List(Of String)
    Public CurrentOrientation As Tile.eOrientation
    Dim WithEvents CurrentMap As MapPage


    Private Enum eViewToggle
        Large
        Medium
        Small
        List
    End Enum
    Private ViewToggle As eViewToggle = eViewToggle.Medium

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.A Then
            CurrentOrientation += 1
            If CurrentOrientation > Tile.eOrientation.Unspecified Then CurrentOrientation -= Tile.eOrientation.Unspecified
            Dim b As Image = SplitContainer2.Panel2.BackgroundImage
            b.RotateFlip(RotateFlipType.Rotate90FlipNone)
            SplitContainer2.Panel2.BackgroundImage = b
            SplitContainer2.Panel2.Invalidate()
        End If
        If e.KeyCode = Keys.D Then
            CurrentOrientation -= 1
            If CurrentOrientation < 0 Then CurrentOrientation += Tile.eOrientation.Unspecified
            Dim b As Image = SplitContainer2.Panel2.BackgroundImage
            b.RotateFlip(RotateFlipType.Rotate270FlipNone)
            SplitContainer2.Panel2.BackgroundImage = b
            SplitContainer2.Panel2.Invalidate()
        End If
        If Not TabControl1.SelectedTab Is Nothing AndAlso Not CType(TabControl1.SelectedTab, MapPage).ActiveTile Is Nothing Then
            With CType(TabControl1.SelectedTab, MapPage).ActiveTile
                .Orientation = CurrentOrientation
                .TileImage = SplitContainer2.Panel2.BackgroundImage
            End With
        End If
        For Each M As MapPage In Me.TabControl1.TabPages
            M.HalfGrid = e.Shift
        Next

    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        For Each M As MapPage In Me.TabControl1.TabPages
            M.HalfGrid = e.Shift
        Next
    End Sub


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        'ListView1.Items.Add("Ruin", 0)
        ListView1.LargeImageList = MediumImageList
        ListView1.View = View.LargeIcon
        ListView1.LabelEdit = False
        Dim m As New MapPage
        TabControl1.TabPages.Add(m)
        TabControl1.SelectedTab = m
        CurrentMap = m
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        With ListView1
            Select Case ViewToggle
                Case eViewToggle.List
                    .LargeImageList = LargeImageList
                    .View = View.LargeIcon
                    ViewToggle = eViewToggle.Large
                Case eViewToggle.Large
                    .LargeImageList = MediumImageList
                    .View = View.LargeIcon
                    ViewToggle = eViewToggle.Medium
                Case eViewToggle.Medium
                    .View = View.SmallIcon
                    ViewToggle = eViewToggle.Small
                Case Else
                    .View = View.List
                    ViewToggle = eViewToggle.List
            End Select
        End With

    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        If ListView1.SelectedItems.Count > 0 Then
            Dim i As Int32 = ListView1.SelectedItems.Item(0).ImageIndex
            SplitContainer2.Panel2.BackgroundImage = pvtTileset.Items(i).TileImage
            CType(TabControl1.SelectedTab, MapPage).ActiveTile = New Tile(pvtTileset.Items(i))
        End If
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        pvtTileset.LoadTiles()
        SmallImageList.Images.Clear()
        LargeImageList.Images.Clear()
        MediumImageList.Images.Clear()
        PaletteKey.Clear()
        ListView1.Items.Clear()
        Dim i As Int32 = 0
        For Each t2 As Tile In pvtTileset.Items
            LargeImageList.Images.Add(t2.PaletteImage)
            SmallImageList.Images.Add(t2.PaletteImage)
            MediumImageList.Images.Add(t2.PaletteImage)
            PaletteKey.Add(t2.Name)
            ListView1.Items.Add(t2.Name, i)
            i += 1
        Next



        SetStatusText("Ready.")
    End Sub

    Private Sub TabControl1_Deselecting(sender As Object, e As TabControlCancelEventArgs) Handles TabControl1.Deselecting
        CurrentMap = TabControl1.SelectedTab
    End Sub

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        With SaveFileDialog1
            .Filter = "Dungeon Tile Map|*.mdm"
            .Title = "Save a Dungeon Tile Map File"
            Dim dlgres As DialogResult = .ShowDialog()
            If dlgres = Windows.Forms.DialogResult.OK AndAlso .FileName <> "" Then
                Dim formatter As New Xml.Serialization.XmlSerializer(GetType(Map))
                Dim stream As FileStream = .OpenFile()
                formatter.Serialize(stream, CType(TabControl1.SelectedTab, MapPage).Map)
                stream.Close()
            End If
            SetStatusText("File saved.")
        End With
    End Sub

    Public Sub SetStatusText(ByVal value As String)
        ToolStripStatusLabel1.Text = value
    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        With OpenFileDialog1
            .Filter = "Dungeon Tile Map|*.mdm"
            .Title = "Open a Dungeon Tile Map File"
            Dim dlgres As DialogResult = .ShowDialog()
            If dlgres = Windows.Forms.DialogResult.OK AndAlso .FileName <> "" Then
                Dim formatter As New Xml.Serialization.XmlSerializer(GetType(MapPage))
                Dim stream As FileStream = .OpenFile()
                Dim m As Map = formatter.Deserialize(stream)
                Dim mp As New MapPage
                mp.Map = m
                TabControl1.TabPages.Add(mp)
                TabControl1.SelectedTab = mp
            End If
        End With
    End Sub
End Class

