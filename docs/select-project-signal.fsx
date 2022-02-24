(**
---
title: Select Project Signals
category: Assignments
categoryindex: 2
index: 4
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)


# Submitting your trading signal preferences.
You need to select your main trading signal. 
Everybody will examine a different signal, 
so you need to give me an ordered list of your preferred signals. 

I will randomly iterate through students and 
assign them their top choice from among the signals remaining.

The signals are listed in the "Project signals list.xlsx" file
that you can find on moodle. 
    - Use the **VARIABLE** column to identify 
      the variable that you want. 
    - Do not use any other columns to identify your variable. 
    - Only variables from that excel file are valid.

You will submit your choices as a .csv file. Here's how to do that.

Use the [FSharp.Data CsvProvider](https://fsprojects.github.io/FSharp.Data/library/CsvProvider.html) 
to construct your csv file. 
All the csv related stuff below comes from things discussed somewhere on that webpage.

First construct a sample, then use the type provider 
to define the type from the sample.

Below you will find a variable named "signals".
Do not modify it. This is to check that you
do not have a typo in your preferred signals.
The code that you use starts after the
"Code starts now" section.
*)

//****************************************
//*****Code starts below this section*****
//****************************************
// Do not modify the signals variable
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
    |> set 

//****************************************
//****Code starts now*********************
//****************************************

(**
*)
#r "nuget:FSharp.Data"

open FSharp.Data
[<Literal>]
let submissionCsvSample = "StudentId (int), Preference (int), Signal (string)"

type SubmissionCsv = CsvProvider<submissionCsvSample>

(** What are your (ordered) preferences?

1. Fill in with your student id so that I know who you are.
*)
let myStudentId = 007

// check that you are paying attention:
if myStudentId = 007 then failwith "Are you sure that 007 is your student id number?"

(** 2. Fill in with your preferences *)
let myPreferences =
    // **Make sure you use the exact string from the VARIABLE column**
    // do as many choices as you want.
    // If you give me 3 and you run out of choices because
    // everybody else already took your top 3 choices,
    // then I will just randomly assign you something.
    //
    // So if you care it's probably best to give more choices.
    [
        "eqpo_me" // first choice **Use VARIABLE, not the name "Payout Yield"**
        "eqnpo_me"// second choice
        "sale_gr3"// third choice
        "sale_gr1"// fourth choice
    ]

(** 
This code checks if you have a typo in myPreferences
Run it. If there's typo you will see an exception.
*)
let typos =
    //[|"eqpo_me1";"eqpo_me2"|]
    myPreferences 
    |> List.filter (fun signal -> not (signals.Contains signal))
if not (List.isEmpty typos) then 
    let typos = typos |> String.concat " and "
    failwith ($"{typos} is not a signal. Check spelling.")
else printfn "%s" "Congrats, no typos"

let rowCreator i signal =
    // Csv types have a member .Row that constructs a row.
    // Technically, the Csv is a class and .Row is a member of that
    // class. But none of this matters. Just think of .Row as a
    // function that goes with the csv type that allows you to use
    // named parameters.
    //
    // if you hover over ".Row" below, you can see what the columns of the row
    // are (studentId,preference,signal). Intellisense should also
    // pop them up for you if you were typing it from scratch. Such
    // as delete the comma from "myStudentId," and then retype the comma.
    // you should see a popup with the column info saying what is in
    // a row.
    SubmissionCsv.Row
        (studentId=myStudentId,
         preference=i,
         signal=signal)

// test
let test1 = rowCreator 0 "sale_gr1"
test1.StudentId
test1.Signal
test1.Preference

(**
We'll use List.mapi to create our rows.
List.mapi is like List.map, but it adds
the index of the list (i)
as a parameter to the function. 
This is useful when you want to use the index 
of the element in the list
as part of your function.

Here, we've ordered our list myPreferences
based on our first, second, 3rd choice etc.
So we're using the index of the element in the list as our
preference column.

0 is first choice, 1 is second choice, 2 is 3rd choice ... 
*)
let mySubmission =
    myPreferences
    // What we have below is the same as
    // |> List.mapi(fun i x -> rowCreator i x)
    //
    // but because we wrote rowCreator to accept i and x
    // as the first two function inputs, we can 
    // take the shortcut and do below.
    |> List.mapi rowCreator

(**
This is an array of rows.

Check your first preference
*)
let myFirstPreference = mySubmission |> List.head

myFirstPreference.StudentId // make sure it's not 7
myFirstPreference.Preference // should be 0
myFirstPreference.Signal // choose wisely

(*
Rather than an "list of csv rows", we want a "csv file".

Here's how we do that. We pass the SubmissionCsv.Rows contained
in the mySubmission array  to the
SubmissionCsv type constructor like so.
*)
let mySubmissionFile = new SubmissionCsv(mySubmission)

(**
Ok, let's write the file. Remember that `__SOURCE_DIRECTORY__`
is a magic variable that points to whatever folder this script file
(aka the source code file) is contained in.

So this will write a csv file in the current directory.
*)

let fileName = $"{myStudentId}.csv"
let fullFilePath = $"{__SOURCE_DIRECTORY__}/{fileName}"

mySubmissionFile.Save(fullFilePath)

(**
Now find that file and submit it. Also click on it in visual studio code;
you should be able to see what the file looks like. It's just a text file with a .csv extension.
*)

(**
and if you want to read the data back in from the file to see that it works:
*)
let backIn = SubmissionCsv.Load(fullFilePath)

backIn.Rows
|> Seq.toList
|> List.map(fun row -> row.Preference, row.Signal)
