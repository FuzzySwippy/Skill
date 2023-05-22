using System.Diagnostics;

namespace Skill;

static class Program
{
	static void Main(string[] args)
	{ 
		if (args.Length < 2)
		{
			Console.WriteLine("Usage: skill <start delay> <process name> <process name> ...");
			Environment.Exit(1);
			return;
		}

		//Check if the user is root
		if (Environment.OSVersion.Platform != PlatformID.Unix)
		{
			Console.WriteLine("This program can only be run on Linux.");
			Environment.Exit(1);
			return;
		}

		if (Environment.GetEnvironmentVariable("USER") != "root")
		{
			Console.WriteLine("This program requires root privileges.");
			Environment.Exit(1);
			return;
		}

		//Check if the first argument is a number
		if (!int.TryParse(args[0], out int delay))
		{
			Console.WriteLine("The first argument must be a number specifying a delay in milliseconds.");
			Environment.Exit(1);
			return;
		}

		//ToLower all args
		for (int i = 1; i < args.Length; i++)
			args[i] = args[i].ToLower();

		KillProcesses(delay, args[1..]);
	}

	static void KillProcesses(int delay, string[] nameParts)
	{
		Thread.Sleep(delay);

		Process[] processes = Process.GetProcesses();
		int killed = 0;

		foreach (Process process in processes)
		{
			if (NameHasParts(nameParts, process.ProcessName))
			{
				try
				{
					process.Kill();
					killed++;
					Console.WriteLine($"Killed process: {process.ProcessName}.");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to kill process {process.ProcessName}: {ex.Message}");
				}
			}
		}

		Console.WriteLine($"Killed {killed} processes.");
	}

	static bool NameHasParts(string[] nameParts, string name)
	{
		name = name.ToLower();

		foreach (string part in nameParts)
			if (name.Contains(part))
				return true;

		return false;
	}
}