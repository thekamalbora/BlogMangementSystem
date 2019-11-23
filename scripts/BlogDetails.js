var GlobalURL = "http://localhost:17197/";

$(document).ready()
{
    ShowUserOppinion();
    ShowCategories();
    ShowBlogDetailsByBlogID();
  
  
}


$(".sliding-link").click(function (e) {
    e.preventDefault();
    var target = $(this).attr('href');
    $(target).scrollTop($(target)[0].scrollHeight);
});


function ShowBlogDetailsByBlogID() {

    $.ajax({
        url: GlobalURL + "BlogMaster/ShowBlogDetailsByBlogID",
        type: "POST",
        data: { BlogID: $("#hfBlogID").val() },
        success: function (data) {
            debugger;
            $("#BlogDetail").html(data.Grid);
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




function ShowUserOppinion() {
    $.ajax({
        url: GlobalURL + "BlogMaster/ShowUserOppinion",
        type: "POST",
        data: { BlogID: $("#hfBlogID").val() },
        success: function (data) {
            $("#ShowBlogOpenion").html(data.Grid);

        },
        error: function () {
            
        }
    })
}
