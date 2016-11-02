option explicit

Dim newExe: newExe = "TsGui.exe"
Dim referenceExe: referenceExe = "TsGu-0.9.4.0.exe"

Dim objShell: Set objShell = CreateObject("Wscript.Shell")
Dim objFSO: Set objFSO = CreateObject("Scripting.FileSystemObject")
Dim objScriptFile: Set objScriptFile = objFSO.GetFile(Wscript.ScriptFullName)
Dim strScriptParentFolder: strScriptParentFolder = objScriptFile.ParentFolder
Dim objScriptParentFolder: set objScriptParentFolder = objFSO.GetFolder(strScriptParentFolder)

Dim file

'objShell.Run strPath

For Each file In objScriptParentFolder.Files
	if file.Type = "XML Document" then
		wscript.echo file.name
		objFSO.CopyFile file.path,file.ParentFolder & "\\Config.xml"
		objShell.Run strScriptParentFolder & "\\" & newExe
		objShell.Run strScriptParentFolder & "\\" & referenceExe
		
		WScript.Echo "Press [ENTER] to continue..."
		' Read dummy input. This call will not return until [ENTER] is pressed.
		WScript.StdIn.ReadLine
	end if
Next