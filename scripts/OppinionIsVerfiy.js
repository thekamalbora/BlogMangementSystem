var GlobalURL = "http://localhost:17197/";

$(document).ready()
{
    GetOpinionRecord();
    IsVerifiedOpinion();
    GetOpinionRecordIsVerified();
}

function GetOpinionRecord() {
    $.ajax({
        url: GlobalURL + "BlogMaster/GetOpinionRecord",
        type: "POST",
        data: {},
        success: function (data) {
            $("#lbOppinionDetails").html(data.Grid);
            $("#totalrecord").html(data.RowCount);
        },
        error: function () {
            alert("Record Not Load");
        }
    })
}

function OpinionDataDelete(OppinionID) {
    if (confirm("Are you sure to delete!!")) {
        $.ajax({
            url: GlobalURL + "BlogMaster/OpinionDataDelete",
            type: "POST",
            data: { OppinionID: OppinionID },
            success: function () {
                alert("Record Deleted Successfully");
                GetAllBlog();
            },
            error: function () {
                alert("Record Not Deleted");
            }
        });
    }
}

function OpinionEditRecord(OppinionID) {
    $.ajax({
        url: GlobalURL + "BlogMaster/OpinionEditRecord",
        type: "POST",
        data: { OppinionID: OppinionID },
        success: function (data) {
            data = JSON.parse(data);
            $("#hfBlogID").val(BlogID);
            $("#txtBlogTitle").val(data[0].BlogTitle);
            $("#txtBlogDiscription").val(data[0].BlogDiscription);
            $("#txtBlogSequence").val(data[0].BlogSequence);
            $("#txtSubmisionDate").val(data[0].SubmissionDate);
            $("#ddlBlogCategory").val(data[0].CatID);

            if ($("#ChkIsShow").val() == true) {
                $("#ChkIsShow").prop("checked", true);
            }
            else {
                $("#ChkIsShow").attr("checked", false);
            }
            $("#txtBlogDetailTitle").val(data[0].BlogDetailTittle);

            $("#txtBlogFullDetailed").Editor("setText", data[0].BlogDetails);



            $("#btnPacketSave").val("Update");
        },
        error: function () {
            alert("Record not Edited");
        }
    });
}


$("#btnIsVerify").click(function () {


    var checked = [];
    var OppinionIdStore=[];
    $('[name="chkdverify"]:checked').each(function (i, e) {
        checked.push($(this).val());
        OppinionIdStore = checked.join(',');
      
    });
    $.ajax({
        url: GlobalURL + "BlogMaster/OppinionIsVerifyByID",
        type: "POST",
        data: { ItemIds: OppinionIdStore },
        success: function (data) {
            alert("Verified Sucessfull");
        },
        error: function () {
            alert("Record Not Load");
        }
    })

  
});



function GetOpinionRecordIsVerified() {
    $.ajax({
        url: GlobalURL + "BlogMaster/GetOpinionRecordIsVerified",
        type: "POST",
        data: {},
        success: function (data) {
            $("#lbOppinionDetailsIsVerified").html(data.Grid);
            $("#totalrecordisverified").html(data.RowCount);
        },
        error: function () {
            alert("Record Not Load");
        }
    })
}

function IsVerifiedOpinion()
{
    //$("#chckIsVerified").on("click", function () {
    //    $("#div2").toggle(this.checked);
    //    $("#div1").hide();
    //});

    $('#chckIsVerified').change(function () {
        if (this.checked)
        {
            $('#div2').fadeIn('slow');
            $('#div1').fadeOut('slow');
        }
            
        else
        {
            $('#div1').fadeIn('slow');
            $('#div2').fadeOut('slow');
        }
          

    });
}