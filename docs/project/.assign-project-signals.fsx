#r "nuget:FSharp.Data"
open System
open System.IO

open FSharp.Data

Environment.CurrentDirectory <- @"C:\Users\Nicholas Hirschey\Downloads"
let [<Literal>] ResolutionFolder = @"C:\Users\Nicholas Hirschey\Downloads"

let input = "signal-choices"
let [<Literal>] studentList = "202223_S2_2280_all_roster.csv"

let files =
    let files = 
        Directory.GetFiles(
        input, 
        searchPattern="*.csv",
        searchOption=SearchOption.AllDirectories
        )
    [ for file in files do
        Path.Combine(Environment.CurrentDirectory,
                     file) ]
    
[<Literal>]
let submissionCsvSample = 
    "StudentId,Preference,Signal
    000,1,signal1
    000,2,signal2"

// I need to allow multiple possible separators because of
// different european locales. Some students will default
// to using ";" when writing csv files because their country
// uses "," for floats as the decimal point.
type SubmissionCsv = CsvProvider<
    submissionCsvSample,
    Separators=";,",
    ResolutionFolder=__SOURCE_DIRECTORY__>
let x = SubmissionCsv.GetSample()

type Signal = Signal of string
type StudentSubmisson =
    { Student : int
      Preferences : Signal list }      
let submissions =
    files
    |> List.map(fun file ->
        let preferences =
            SubmissionCsv.Load(file).Rows
            |> Seq.sortBy(fun x -> x.Preference)
            |> Seq.toList
        { Student = preferences.[0].StudentId
          Preferences = 
            preferences
            |> List.map(fun x -> Signal x.Signal) })
    |> List.filter(fun x -> x.Student <> 007)

let signals =
    ["niq_su"
     "ret_6_1"
     "ret_12_1"
     "saleq_su"
     "tax_gr1a"
     "ni_inc8q"
     "prc_highprc_252d"
     "resff3_6_1"
     "resff3_12_1"
     "be_me"
     "debt_me"
     "at_me"
     "ret_60_12"
     "ni_me"
     "fcf_me"
     "div12m_me"
     "eqpo_me"
     "eqnpo_me"
     "sale_gr3"
     "sale_gr1"
     "ebitda_mev"
     "sale_me"
     "ocf_me"
     "intrinsic_value"
     "bev_mev"
     "netdebt_me"
     "eq_dur"
     "capex_abn"
     "at_gr1"
     "ppeinv_gr1a"
     "noa_at"
     "noa_gr1a"
     "lnoa_gr1a"
     "capx_gr1"
     "capx_gr2"
     "capx_gr3"
     "chcsho_12m"
     "eqnpo_12m"
     "debt_gr3"
     "inv_gr1"
     "inv_gr1a"
     "oaccruals_at"
     "taccruals_at"
     "cowc_gr1a"
     "coa_gr1a"
     "col_gr1a"
     "nncoa_gr1a"
     "ncoa_gr1a"
     "ncol_gr1a"
     "nfna_gr1a"
     "sti_gr1a"
     "lti_gr1a"
     "fnl_gr1a"
     "be_gr1a"
     "oaccruals_ni"
     "taccruals_ni"
     "netis_at"
     "eqnetis_at"
     "dbnetis_at"
     "niq_be"
     "niq_be_chg1"
     "niq_at"
     "niq_at_chg1"
     "ebit_bev"
     "ebit_sale"
     "sale_bev"
     "at_turnover"
     "gp_at"
     "gp_atl1"
     "ope_be"
     "ope_bel1"
     "op_at"
     "op_atl1"
     "cop_at"
     "cop_atl1"
     "f_score"
     "o_score"
     "z_score"
     "pi_nix"
     "at_be"
     "saleq_gr1"
     "rd_me"
     "rd_sale"
     "opex_at"
     "emp_gr1"
     "rd5_at"
     "age"
     "dsale_dinv"
     "dsale_drec"
     "dgp_dsale"
     "dsale_dsga"
     "sale_emp_gr1"
     "tangibility"
     "kz_index"
     "ocfq_saleq_std"
     "cash_at"
     "ni_ar1"
     "ni_ivol"
     "earnings_variability"
     "aliq_at"
     "aliq_mat"
     "seas_1_1an"
     "seas_1_1na"
     "seas_2_5an"
     "seas_2_5na"
     "seas_6_10an"
     "seas_6_10na"
     "seas_11_15an"
     "seas_11_15na"
     "seas_16_20an"
     "seas_16_20na"
     "market_equity"
     "ivol_ff3_21d"
     "ivol_capm_252d"
     "ivol_capm_21d"
     "ivol_hxz4_21d"
     "rvol_21d"
     "beta_60m"
     "betabab_1260d"
     "beta_dimson_21d"
     "turnover_126d"
     "turnover_var_126d"
     "dolvol_126d"
     "dolvol_var_126d"
     "prc"
     "ami_126d"
     "zero_trades_21d"
     "zero_trades_126d"
     "zero_trades_252d"
     "rmax1_21d"
     "rskew_21d"
     "iskew_capm_21d"
     "iskew_ff3_21d"
     "iskew_hxz4_21d"
     "coskew_21d"
     "ret_1_0"
     "betadown_252d"
     "bidaskhl_21d"
     "ret_3_1"
     "ret_9_1"
     "ret_12_7"
     "corr_1260d"
     "rmax5_21d"
     "rmax5_rvol_21d"
     "ni_be"
     "ocf_at"
     "ocf_at_chg1"
     "mispricing_perf"
     "mispricing_mgmt"
     "qmj"
     "qmj_prof"
     "qmj_growth"
     "qmj_safety"]
    |> List.map Signal 

let misnamed, _correctlyNamed =
    // using set because this is fast look-ups for when
    // the key is the same thing as the value.
    let setSignals = set signals
    submissions
    // In the following lambda expression, I'm going to pattern match part of a record
    // I can do this because I have only one record with a preferences field,
    // so the record that I am matching is unambiguous
    |> List.collect(fun { Preferences = prfs} -> prfs)
    |> List.distinct 
    |> List.partition(fun signal -> not (setSignals.Contains signal))
let setOfMisnamed = set misnamed

submissions
|> List.filter(fun x -> 
    x.Preferences
    |> List.exists(fun y -> setOfMisnamed.Contains y))


misnamed
|> List.iter (printfn "%A, Signal \"CORRECTED\"")

let corrections =
    [ Signal "saleq_gr", Signal "saleq_gr1"
      Signal "ebitda at", Signal "ebitda_mev"
      Signal "ret 6 1", Signal "ret_6_1"
      Signal "fcf gr3a", Signal "ocf_me"
      Signal "fcf*", Signal "fcf_me"
      Signal "Price momentum t-3 to t-1", Signal "ret_3_1"
      Signal "Price momentum t-6 to t-1", Signal "ret_6_1"
      Signal "Price momentum t-9 to t-1", Signal "ret_9_1"
      Signal "Pitroski F-score", Signal "f_score"
      Signal "Altman Z-score", Signal "z_score"
      Signal "Ohlson O-score", Signal "o_score"
      Signal "Sales Growth (1 year)", Signal "sale_gr1"
      Signal "Sales growth (1 quarter)", Signal "saleq_gr1"
      Signal "Earnings-to-price", Signal "ni_me"
      Signal "Return on equity", Signal "ni_be"
      Signal "rmax5_rvol_21", Signal "rmax5_rvol_21d"
      Signal "mispricing_mgm", Signal "mispricing_mgmt" 
      Signal "rvol 21d", Signal "rvol_21d"
      Signal "at gr1", Signal "at_gr1"
      Signal "sale gr1", Signal "sale_gr1"
      Signal "sale gr3", Signal "sale_gr3"
      Signal "saleq gr1", Signal "saleq_gr1"
      Signal "ivol capm 252d", Signal "ivol_capm_252d"
      Signal "ivol ff3 21d", Signal "ivol_ff3_21d"
      Signal "z score", Signal "z_score"
      Signal "qmj growth", Signal "qmj_growth"
      Signal "qmj prof", Signal "qmj_prof"
      Signal "ocfq saleq std", Signal "ocfq_saleq_std"
      Signal "earnings variabilityF", Signal "earnings_variability"
      Signal "noa at", Signal "noa_at"
      Signal "ni ivol", Signal "ni_ivol"
      Signal "rmax5 21d", Signal "rmax5_21d"
      Signal "rmax1 21d", Signal "rmax1_21d"
      Signal "seas 1 1an", Signal "seas_1_1an"
      Signal "capx gr2", Signal "capx_gr2"
      Signal "capx gr3", Signal "capx_gr3"
      Signal "capx gr1", Signal "capx_gr1"
      Signal "seas 2 5na",Signal "seas_2_5na"
      Signal "ni inc8q", Signal "ni_inc8q"
      Signal "rmax5 rvol 21d", Signal "rmax5_rvol_21d"
      Signal "inv gr1", Signal "inv_gr1"
      Signal "beta 60m", Signal "beta_60m" ]
    |> Map

let removeRemainingMisspelled = false
let correctedSubmissions =
    let corrected = 
        submissions
        |> List.map(fun x ->
            let newPrefs =
                x.Preferences
                |> List.map(fun x ->
                    match Map.tryFind x corrections with
                    | None -> x
                    | Some correction -> correction )
            { x with Preferences = newPrefs })

    if removeRemainingMisspelled then // running this will toss out any uncorrected typos.
        corrected
        |> List.map(fun x ->
            { x with 
                Preferences = 
                    x.Preferences
                    |> List.filter(fun y -> List.contains y signals )} )
    else corrected

let checkCorrected =
    correctedSubmissions
    |> List.collect(fun x -> x.Preferences)
    |> List.filter(fun x -> not (List.contains x signals))
if not (List.isEmpty checkCorrected) then failwith "You still have missing signals"

correctedSubmissions
|> List.collect(fun x -> x.Preferences)
|> List.countBy id
|> List.sortByDescending snd

// Now write the code to make assignments.

let rec assignStudentSignal 
    (remainingSignals: Signal list) 
    (studentRequests: Signal list) =
    let remainingSignals = List.distinct remainingSignals    
    match studentRequests with
    | firstRequest::remainingRequests ->
        let matchedVsNotMatched = remainingSignals |> List.partition(fun x -> x = firstRequest) 
        match matchedVsNotMatched with
        | [], notMatched -> assignStudentSignal notMatched remainingRequests
        | matched::[], notMatched -> matched, notMatched  
        | _ -> failwith $"you probably have duplicated signals in {nameof(remainingSignals)}"
    | [] -> 
        match remainingSignals with
        | [] -> failwith "no signals remain to assign"
        | selected::notSelected -> selected, notSelected

assignStudentSignal [Signal "a";Signal "b";Signal "c"] [Signal "b";Signal "c"]
assignStudentSignal [Signal "a";Signal "b";Signal "c"] [Signal "d";Signal "c"]
assignStudentSignal [Signal "a";Signal "b";Signal "c"] [Signal "e";Signal "f"]
assignStudentSignal [Signal "a";Signal "a";Signal "c"] [Signal "e";Signal "f"]

type StudentAssignment = 
    { Student : int
      Assignment : Signal }

let rec assignSignals 
    (remainingSignals: Signal list) 
    (requests: StudentSubmisson list) 
    (assignments: StudentAssignment list) =

    match requests with
    | [] -> assignments, remainingSignals
    | thisStudent::remainingStudents ->
        let selectedSignal, notSelectedSignals = 
            assignStudentSignal remainingSignals thisStudent.Preferences 
        let thisStudentAssignment =
            { Student = thisStudent.Student
              Assignment = selectedSignal }
        assignSignals 
            notSelectedSignals 
            remainingStudents 
            (thisStudentAssignment::assignments)

let rand = new Random(100)

let randomOrderSubmissions =
    correctedSubmissions
    |> List.sortBy(fun x -> rand.NextDouble())


let assignedSubmitted, remainingSignalsAfterSubmitted = assignSignals signals randomOrderSubmissions []

let studentListFile = CsvProvider<studentList,ResolutionFolder=ResolutionFolder>.GetSample()
let didNotSubmit =
    let submitted = assignedSubmitted |> List.map(fun x -> x.Student) |> set
    studentListFile.Rows
    |> Seq.map(fun x -> x.``STUDENT ID``)
    |> Seq.filter(fun id -> not (submitted.Contains id))     
    |> Seq.map(fun id -> { Student = id; Preferences = []})
    |> Seq.toList

let allAssignments, remainingSignals = assignSignals remainingSignalsAfterSubmitted didNotSubmit assignedSubmitted


// some checks
let nUniqueAssignments = 
    allAssignments
    |> List.map(fun x -> x.Assignment)
    |> List.distinct
    |> List.length
let nUniqueStudents = 
    allAssignments
    |> List.map(fun x -> x.Student)
    |> List.distinct
    |> List.length
let nShouldBe = (submissions.Length + didNotSubmit.Length)
if nUniqueAssignments <> nShouldBe ||
   nUniqueStudents <> nShouldBe  
then failwith "mismatched assignment numbers"

// write the file
type AssignmentCsv = CsvProvider<"StudentId (int),Signal (string)">

let assignmentFile =
    allAssignments
    |> List.map(fun x ->
        let (Signal signal) = x.Assignment
        AssignmentCsv.Row(studentId=x.Student,signal=signal))
    |> fun rows -> new AssignmentCsv(rows)
assignmentFile.Save("assignedSignals.csv")        
