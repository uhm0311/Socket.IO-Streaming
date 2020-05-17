@echo off

set argc=0
for %%x in (%*) do Set /A argc+=1

if %argc% GEQ 1 (
	node "%1" %2
) else (
	echo Usage : launch.bat "Javascript File Path" [Port Number]
)

pause