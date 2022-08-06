namespace DataContracts
{
    public class JobeSubmissionRequest
    {
        public RunSpec run_spec { get; set; }
    }
    
    public class RunSpec
    {
        public string language_id { get; set; }

        public string input { get; set; }

        public string sourcefilename { get; set; }

        public string sourcecode { get; set; }

        public Parameters parameters { get; set; }
    }

    public class Parameters
    {
        public int? disklimit { get; set; }

        public int? memorylimit { get; set; }

        public int? streamsize { get; set; }

        public int? cputime { get; set; }

        public int? numprocs { get; set; }
    }
}
