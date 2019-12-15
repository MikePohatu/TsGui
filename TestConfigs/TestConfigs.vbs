option explicit

' Dim newExe: newExe = "TsGui.exe"
' Dim referenceExe: referenceExe = "TsGui.1.0.0.0.exe"

Dim objShell: Set objShell = CreateObject("Wscript.Shell")
Dim objFSO: Set objFSO = CreateObject("Scripting.FileSystemObject")
Dim objScriptFile: Set objScriptFile = objFSO.GetFile(Wscript.ScriptFullName)
Dim strScriptParentFolder: strScriptParentFolder = objScriptFile.ParentFolder
Dim objScriptParentFolder: set objScriptParentFolder = objFSO.GetFolder(strScriptParentFolder)

Dim intFileNumber
Dim file, currentfile

'objShell.Run strPath
WScript.Echo "Which number config do you want to start with: "
' Read dummy input. This call will not return until [ENTER] is pressed.
intFileNumber = WScript.StdIn.ReadLine()
		

do while true
	currentfile = strScriptParentFolder & "\Config." & intFileNumber & ".xml"
	if objFSO.FileExists(currentfile) then 
		SET file = objFSO.GetFile(currentfile)
		wscript.echo currentfile

		if (file.Type = "XML Document") then
			
			'objFSO.CopyFile file.path,file.ParentFolder & "\\Reference\\Config.xml"
			'objFSO.CopyFile file.path,file.ParentFolder & "\\Test\\Config.xml"

			wscript.echo "Launching reference: " & file.name
			objShell.Run strScriptParentFolder & "\\Reference\\TsGui.exe -config " &  file.path
			WScript.Echo "Press [ENTER] to continue..."
			WScript.StdIn.ReadLine

			wscript.echo "Launching test: " & file.name
			objShell.Run strScriptParentFolder & "\\Test\\TsGui.exe -config " &  file.path
			WScript.Echo "Press [ENTER] to continue..."
			WScript.StdIn.ReadLine
			
		end if
	else 
		wscript.echo "File not found, exiting: " & currentfile
		exit do
	end if
	intFileNumber = intFileNumber + 1
loop