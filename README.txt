..::: DESCRIPTION ::..
Cloned via Subversion from source -> https://hl7analyst.codeplex.com/
Author: Jeremy Reagan
Last Release: 10/1/2011
Editor: Chris Ryan
Date Cloned: 2/8/2017

..::: ORIGINAL DESCRIPTION ::..
Project Description 
HL7 Analyst allows users to view, edit, and save Version 2.x HL7 messages. You'll never have to count pipe characters in notepad again. HL7 Analyst is developed in C# 4.0 for the windows platform and is released under the GPL V2 Open Source License. 

Imagine a world where looking at HL7 doesn't make your eyes hurt, where anyone can look at and understand an HL7 message without the need for high priced professionals. HL7 Analyst can make this dream a reality by putting key analysis features at your fingertips. See the list of features on the documentation page to find out how HL7 Analyst can change your world today!
Blurb about HL7 Analyst

My day before I built HL7 Analyst consisted of using multiple pieces of software to locate and analyze HL7 records. To help reduce the amount of software I had to keep open and running all the time, I built HL7 Analyst. I’ve tried many other free HL7 Analysis tools, and none of them contain as many features as this one little tool. Sure, there are better and more powerful out there, but you’ll be paying a considerable amount for them. For a really great HL7 Analysis tool that you have to pay for, check out HL7 Spy here. But for those of you that are running on a small to non-existent budget like I do, I hope that you find HL7 Analyst as helpful as I have.

Sincerely,

Jeremy Reagan

Last edited Apr 29, 2011 at 1:09 PM by JReagan_1, version 11

..::ENCOUNTERED ISSUES AND ERRORS::..
>Warning: "Missing XML comment for publicly visible type or member"
	>Occurs within HL7Lib project, Segment.cs, caused by the files within the Segments directory
	>Is a benign warning but can be disabled:
		>Right-click the HL7Lib project and open properties
		>Within the "Build" tab, uncheck the "XML documentation file" box
	>Sections can also be ignored by encasing them with the following:
		>before: #pragma warning disable 1591
		>after:  #pragma warning restore 1591
	>This type of warning can also be ignored globally:
		>Right-click the HL7Lib project and open properties
		>Within the "Build" tab, change the "Warning level" from 4 to 3 (https://msdn.microsoft.com/en-us/library/thxezb7y.aspx)
	>[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.??Usage", "CS1591")]
	>Would be nice to resolve however to self-document the code
>Warning: "Could not resolve this reference. Could not locate the assembly 'ZedGraph'..."
	>Is a 3rd party application linked to within frmAbout.cs, pointing to here: https://sourceforge.net/projects/zedgraph/ or from 
	>Is referenced in a using statement within frmMessageStats.cs which throws the warning
	>Should be replaced in code with a .NET 4+ charting library but can be used by downloading and referencing this dll for .NET 1.1 or 2.0 https://www.codeproject.com/articles/5431/a-flexible-charting-library-for-net
>Build process:
	>"HL7 Analyst" project was not set to build within the solution
		>Right-click solution and open properties
		>Under Configuration Properties -> Configuration, check the "Build" box for "HL7 Analyst" (platform is x86)
	>Error: "Unable to find manifest signing certificate in the certificate store"
	>Warning: "Unable to find code signing certificate in the current user's Windows certificate store. To correct this, either disable signing of the ClickOnce manifest or install the certificate into the certificate store."
		>This warning is fairly well explained in the message
		>Right-click the "HL7 Analyst" project and open properties
		>On the "Signing" tab, uncheck the "Sign the ClickOnce manifests" box
