var GlobalURL = "http://localhost:17197/";

$(document).ready()
{
}
function SaveOpinion() {
    $.ajax({
        url: GlobalURL + "BlogMaster/OppinionInsertUpdateData",
        type: "POST",
        data:
            {
                OppinionID: $("#hfOppinionID").val(),
                Oppinions: $("#txtOpinion").val(),
                UserName: $("#txtName").val(),
                Email: $("#txtEmailID").val(),
                BlogID: $("#hfBlogID").val(),
            },
        success: function (data) {
            data = data.d;
            alert("Save Data Succesfully");
            clear();
            if (data.Status == "1" || data.Status == "2") {
                
                

                alert(data.Result);


            } else {
                alert(data.Result);
            }
            if (data.Focus != "") {
                $("#" + data.Focus).focus();
            }
        }
    });
}
function clear() {
    $("#hfOppinionID").val(""),
    $("#txtOpinion").val(""),
    $("#txtName").val(""),
    $("#txtEmailID").val(""),
    $("#hfBlogID").val("")
}

