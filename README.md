# HL7 Analyst (Updated)
This is my update to the [HL7 Analyst](https://hl7analyst.codeplex.com/) software originally created by Jeremy Reagan, and released under the [GPL V2 Open Source License](https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html). His last release was on 10/1/2011 and this project was initially cloned on 2/8/2017. This project also utilizes the ZedGraph charting library which is released under the [GPL V2.1 License](https://www.gnu.org/licenses/old-licenses/lgpl-2.1.en.html) and available from [CodeProject](https://www.codeproject.com/Articles/5431/A-flexible-charting-library-for-NET) as well as [SourceForge](https://sourceforge.net/projects/zedgraph/).

## Planned Improvements
1. Update ***frmMain.DeIdentifyMessages()*** method to utilize the ***HL7Lib.Base.DeIdentify()*** method which is currently not in use but does exist
2. Update the ***HL7Lib.Base.DeIdentify()*** method to de-identify messages according to [HIPAA standards](https://www.hhs.gov/hipaa/for-professionals/privacy/special-topics/de-identification/#standard)
3. Update the ***HL7Lib.Base.DeIdentify()*** method to accept customizable randomizations
4. Update the ***HL7Lib.Base.DeIdentify()*** method to accept external configurations for sensitive fields. Useful if messages are non-standard
5. Provide a minimal command line interface for automating the de-identification process
6. Replace the usage of **ZedGraph** with a more up-to-date charting library like [MSChart](https://code.msdn.microsoft.com/mschart)

## Encountered Issues
* **Warning**: *"Missing XML comment for publicly visible type or member"*
    * Occurs within **HL7Lib** project -> */Segment.cs* file and the files within the *Segments* directory
	* Is a benign warning but can be disabled:
		* Right-click the **HL7Lib** project and open properties
		* Within the **Build** tab, uncheck the ***XML documentation file*** box
	* Sections can also be ignored by encasing them with the following:
		* **before:** `#pragma warning disable 1591`
		* **after:** `#pragma warning restore 1591`
	* This type of warning can also be ignored globally (***preferred method***):
		* Right-click the **HL7Lib** project and open properties
		* Within the **Build** tab, change the ***Warning level*** from **4** to **3** ([MSDN](https://msdn.microsoft.com/en-us/library/thxezb7y.aspx))
* **HL7 Analyst** project was not set to build within the solution
	* Right-click solution and open properties
	* Under Configuration Properties -> Configuration, check the ***Build*** box for **HL7 Analyst** (platform is **x86**)
* **Error:** *"Unable to find manifest signing certificate in the certificate store"*
* **Warning:** *"Unable to find code signing certificate in the current user's Windows certificate store. To correct this, either disable signing of the ClickOnce manifest or install the certificate into the certificate store."*
	* This warning is fairly well explained in the message
	* Right-click the **HL7 Analyst** project and open properties
	* On the **Signing** tab, uncheck the ***Sign the ClickOnce manifests*** box
	* This also resolves the related error above

### Original Description ([CodePlex](https://hl7analyst.codeplex.com/))
>Project Description
>
>HL7 Analyst allows users to view, edit, and save Version 2.x HL7 messages. You'll never have to count pipe characters in notepad again. HL7 Analyst is developed in C# 4.0 for the windows platform and is released under the GPL V2 Open Source License. 
>
>Imagine a world where looking at HL7 doesn't make your eyes hurt, where anyone can look at and understand an HL7 message without the need for high priced professionals. HL7 Analyst can make this dream a reality by putting key analysis features at your fingertips. See the list of features on the documentation page to find out how HL7 Analyst can change your world today!
Blurb about HL7 Analyst
>
>My day before I built HL7 Analyst consisted of using multiple pieces of software to locate and analyze HL7 records. To help reduce the amount of software I had to keep open and running all the time, I built HL7 Analyst. I’ve tried many other free HL7 Analysis tools, and none of them contain as many features as this one little tool. Sure, there are better and more powerful out there, but you’ll be paying a considerable amount for them. For a really great HL7 Analysis tool that you have to pay for, check out HL7 Spy here. But for those of you that are running on a small to non-existent budget like I do, I hope that you find HL7 Analyst as helpful as I have.
>
>Sincerely,
>
>Jeremy Reagan
>
>Last edited Apr 29, 2011 at 1:09 PM by JReagan_1, version 11
