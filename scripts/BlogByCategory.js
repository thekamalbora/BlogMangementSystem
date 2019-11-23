var GlobalURL = "http://localhost:17197/";
$(document).ready()
{
    BlogByCategories();
    ShowCategories();
}
function BlogByCategories() {

    $.ajax({
        url: GlobalURL + "BlogMaster/BlogByCategories",
        type: "POST",
        data: { CatID: $("#hfCatID").val() },
        success: function (data) {
            debugger;
            $("#lbPacketDetails").html("");
            $("#lbPacketDetails").html(data.Grid);
        },
        error: function () {
            alert("Record Not Load");
        }
    })
}

function ShowCategories() {
    $.ajax({
        url: GlobalURL + "BlogMaster/ShowCategories",
        type: "POST",
        data: {},
        success: function (data) {
            $("#ShowCategories").html(data.Grid);

        },
        error: function () {
            alert("Record Not Load");
        }
    })
}