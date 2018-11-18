#if NET35
namespace GenerateInterfaces {
    
    
    public partial interface IProcess {
        
        // Int32 BasePriority
        int BasePriority {
            get;
        }
        
        // Int32 ExitCode
        int ExitCode {
            get;
        }
        
        // Boolean HasExited
        bool HasExited {
            get;
        }
        
        // System.DateTime ExitTime
        System.DateTime ExitTime {
            get;
        }
        
        // IntPtr Handle
        System.IntPtr Handle {
            get;
        }
        
        // Int32 HandleCount
        int HandleCount {
            get;
        }
        
        // Int32 Id
        int Id {
            get;
        }
        
        // System.String MachineName
        string MachineName {
            get;
        }
        
        // IntPtr MainWindowHandle
        System.IntPtr MainWindowHandle {
            get;
        }
        
        // System.String MainWindowTitle
        string MainWindowTitle {
            get;
        }
        
        // System.Diagnostics.ProcessModule MainModule
        System.Diagnostics.ProcessModule MainModule {
            get;
        }
        
        // IntPtr MaxWorkingSet
        System.IntPtr MaxWorkingSet {
            get;
            set;
        }
        
        // IntPtr MinWorkingSet
        System.IntPtr MinWorkingSet {
            get;
            set;
        }
        
        // System.Diagnostics.ProcessModuleCollection Modules
        System.Diagnostics.ProcessModuleCollection Modules {
            get;
        }
        
        // Int32 NonpagedSystemMemorySize
        int NonpagedSystemMemorySize {
            get;
        }
        
        // Int64 NonpagedSystemMemorySize64
        long NonpagedSystemMemorySize64 {
            get;
        }
        
        // Int32 PagedMemorySize
        int PagedMemorySize {
            get;
        }
        
        // Int64 PagedMemorySize64
        long PagedMemorySize64 {
            get;
        }
        
        // Int32 PagedSystemMemorySize
        int PagedSystemMemorySize {
            get;
        }
        
        // Int64 PagedSystemMemorySize64
        long PagedSystemMemorySize64 {
            get;
        }
        
        // Int32 PeakPagedMemorySize
        int PeakPagedMemorySize {
            get;
        }
        
        // Int64 PeakPagedMemorySize64
        long PeakPagedMemorySize64 {
            get;
        }
        
        // Int32 PeakWorkingSet
        int PeakWorkingSet {
            get;
        }
        
        // Int64 PeakWorkingSet64
        long PeakWorkingSet64 {
            get;
        }
        
        // Int32 PeakVirtualMemorySize
        int PeakVirtualMemorySize {
            get;
        }
        
        // Int64 PeakVirtualMemorySize64
        long PeakVirtualMemorySize64 {
            get;
        }
        
        // Boolean PriorityBoostEnabled
        bool PriorityBoostEnabled {
            get;
            set;
        }
        
        // System.Diagnostics.ProcessPriorityClass PriorityClass
        System.Diagnostics.ProcessPriorityClass PriorityClass {
            get;
            set;
        }
        
        // Int32 PrivateMemorySize
        int PrivateMemorySize {
            get;
        }
        
        // Int64 PrivateMemorySize64
        long PrivateMemorySize64 {
            get;
        }
        
        // System.TimeSpan PrivilegedProcessorTime
        System.TimeSpan PrivilegedProcessorTime {
            get;
        }
        
        // System.String ProcessName
        string ProcessName {
            get;
        }
        
        // IntPtr ProcessorAffinity
        System.IntPtr ProcessorAffinity {
            get;
            set;
        }
        
        // Boolean Responding
        bool Responding {
            get;
        }
        
        // Int32 SessionId
        int SessionId {
            get;
        }
        
        // System.Diagnostics.ProcessStartInfo StartInfo
        System.Diagnostics.ProcessStartInfo StartInfo {
            get;
            set;
        }
        
        // System.DateTime StartTime
        System.DateTime StartTime {
            get;
        }
        
        // System.ComponentModel.ISynchronizeInvoke SynchronizingObject
        System.ComponentModel.ISynchronizeInvoke SynchronizingObject {
            get;
            set;
        }
        
        // System.Diagnostics.ProcessThreadCollection Threads
        System.Diagnostics.ProcessThreadCollection Threads {
            get;
        }
        
        // System.TimeSpan TotalProcessorTime
        System.TimeSpan TotalProcessorTime {
            get;
        }
        
        // System.TimeSpan UserProcessorTime
        System.TimeSpan UserProcessorTime {
            get;
        }
        
        // Int32 VirtualMemorySize
        int VirtualMemorySize {
            get;
        }
        
        // Int64 VirtualMemorySize64
        long VirtualMemorySize64 {
            get;
        }
        
        // Boolean EnableRaisingEvents
        bool EnableRaisingEvents {
            get;
            set;
        }
        
        // System.IO.StreamWriter StandardInput
        System.IO.StreamWriter StandardInput {
            get;
        }
        
        // System.IO.StreamReader StandardOutput
        System.IO.StreamReader StandardOutput {
            get;
        }
        
        // System.IO.StreamReader StandardError
        System.IO.StreamReader StandardError {
            get;
        }
        
        // Int32 WorkingSet
        int WorkingSet {
            get;
        }
        
        // Int64 WorkingSet64
        long WorkingSet64 {
            get;
        }
        
        // System.ComponentModel.ISite Site
        System.ComponentModel.ISite Site {
            get;
            set;
        }
        
        // System.ComponentModel.IContainer Container
        System.ComponentModel.IContainer Container {
            get;
        }
        
        // Boolean CloseMainWindow()
        bool CloseMainWindow();
        
        // Void Close()
        void Close();
        
        // Void Refresh()
        void Refresh();
        
        // Boolean Start()
        bool Start();
        
        // Void Kill()
        void Kill();
        
        // System.String ToString()
        string ToString();
        
        // Boolean WaitForExit(Int32)
        bool WaitForExit(int milliseconds);
        
        // Void WaitForExit()
        void WaitForExit();
        
        // Boolean WaitForInputIdle(Int32)
        bool WaitForInputIdle(int milliseconds);
        
        // Boolean WaitForInputIdle()
        bool WaitForInputIdle();
        
        // Void BeginOutputReadLine()
        void BeginOutputReadLine();
        
        // Void BeginErrorReadLine()
        void BeginErrorReadLine();
        
        // Void CancelOutputRead()
        void CancelOutputRead();
        
        // Void CancelErrorRead()
        void CancelErrorRead();
        
        // Void Dispose()
        void Dispose();
        
        // System.Object GetLifetimeService()
        object GetLifetimeService();
        
        // System.Object InitializeLifetimeService()
        object InitializeLifetimeService();
        
        // System.Runtime.Remoting.ObjRef CreateObjRef(System.Type)
        System.Runtime.Remoting.ObjRef CreateObjRef(System.Type requestedType);
        
        // Boolean Equals(System.Object)
        bool Equals(object obj);
        
        // Int32 GetHashCode()
        int GetHashCode();
        
        // System.Type GetType()
        System.Type GetType();
    }
}
#endif
#if NET35
namespace GenerateInterfaces {
    
    
    public partial interface ITestObjectAuto {
        
        // Int32 Value
        int Value {
            get;
            set;
        }
        
        // Void DoIt()
        void DoIt();
        
        // Int32 GetValue()
        int GetValue();
        
        // Int32 GetValue(Int32)
        int GetValue(int num);
        
        // Int32 AddValueFromObject(TestClasses.TestObject)
        int AddValueFromObject(TestClasses.TestObject obj);
        
        // Void GetValueOut(Int32 ByRef)
        void GetValueOut(out int val);
        
        // Void GetValueRef(Int32 ByRef)
        void GetValueRef(ref int val);
        
        // Void GetObjectOut(TestClasses.TestObject ByRef)
        void GetObjectOut(out TestClasses.TestObject val);
        
        // T GetFirst[T](System.Collections.Generic.IEnumerable`1[T])
        T GetFirst<T>(System.Collections.Generic.IEnumerable<T> values)
        ;
        
        // T GetObject[T]()
        T GetObject<T>()
            where T : new();
        
        // System.Tuple`2[T,T2] GetObject[T,T2]()
        System.Tuple<T, T2> GetObject<T, T2>()
            where T : new()
            where T2 : new();
        
        // System.Tuple`2[System.Tuple`2[T,T2],System.Tuple`2[T,T2]] GetTupleTuple[T,T2]()
        System.Tuple<System.Tuple<T, T2>, System.Tuple<T, T2>> GetTupleTuple<T, T2>()
            where T : new()
            where T2 : new();
        
        // T GetObjectConstrained[T,T2]()
        T GetObjectConstrained<T, T2>()
            where T :  class, TestClasses.ITypeContraint, T2, new ()
        ;
        
        // TestClasses.TestObject GetNewObject(Boolean)
        TestClasses.TestObject GetNewObject(bool returnNull);
        
        // Int32 AddValuesFromArray(Int32[])
        int AddValuesFromArray(int[] vals);
        
        // Void GetTripleValue(Int32[] ByRef)
        void GetTripleValue(out int[] vals);
        
        // Void FireEvent()
        void FireEvent();
        
        // Int32 AddValuesFromParams(Int32[])
        int AddValuesFromParams(int[] vals);
        
        // System.String ToString()
        string ToString();
        
        // Boolean Equals(System.Object)
        bool Equals(object obj);
        
        // Int32 GetHashCode()
        int GetHashCode();
        
        // System.Type GetType()
        System.Type GetType();
    }
}
#endif
#if NET35
namespace GenerateInterfaces {
    
    
    public partial interface ITestObjectWithGenericTypesAuto<T1, T2>
    
     {
        
        // T1 Value1
        T1 Value1 {
            get;
        }
        
        // T2 Value2
        T2 Value2 {
            get;
        }
        
        // System.String ToString()
        string ToString();
        
        // Boolean Equals(System.Object)
        bool Equals(object obj);
        
        // Int32 GetHashCode()
        int GetHashCode();
        
        // System.Type GetType()
        System.Type GetType();
    }
}
#endif
