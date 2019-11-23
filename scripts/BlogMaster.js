var GlobalURL = "http://localhost:17197/";


$(document).ready()
{
    GetAllBlog();
    GetCategoryRecord();
    ShowBlogDetails();
    ShowCategories();
    Clear();
    GetImage();
    ImageToByte();
    DatePicker();


    //CKEDITOR.replace('editor1', { entities : false, htmlEncodeOutput: true });
    $("#txtBlogFullDetailed").Editor();
}

function DatePicker() {
    $('.datepicker').datepicker({
        format: 'd-M-yyyy',
        todayHighlight: 'TRUE',
        autoclose: true,
    })
}

function ImageToByte() {
    var reader = new FileReader();
    var ImageName;
    var ImageExtension;
    $('input[type=file]').change(function () {
        if (typeof (FileReader) != "undefined") {
            var regex = /^([a-zA-Z0-9\s_\\.\-:])+(.jpg|.jpeg|.gif|.png|.bmp)$/;
            $($(this)[0].files).each(function () {
                var file = $(this);
                if (regex.test(file[0].name.toLowerCase())) {
                    ImageName = file[0].name;
                    ImageExtension = file[0].type;
                    reader.readAsDataURL(file[0]);
                } else {
                    alert(file[0].name + " is not a valid image file.");
                    return false;
                }
            });
        }
        else {
            alert("This browser does not support HTML5 FileReader.");
        }
    });

    $("#btnPacketSave").on("click", function () {
        var BlogImage = reader.result;
        BlogImage = BlogImage.split(';')[1].replace("base64,", "");
        var blogdata = $("#txtBlogFullDetailed").Editor("getText");
        //for (instance in CKEDITOR.instances)
        //{
        //    CKEDITOR.instances[instance].updateElement();
        //}


        $.ajax({
            url: GlobalURL + "BlogMaster/BlogMasterInsertUpdateData",
            type: "POST",
            data:
                {
                    BlogID: $("#hfBlogID").val(),
                    BlogTitle: $("#txtBlogTitle").val(),
                    BlogDiscription: $("#txtBlogDiscription").val(),
                    BlogSequence: $("#txtBlogSequence").val(),
                    SubmissionDate: $("#txtSubmisionDate").val(),
                    CatID: $("#ddlBlogCategory").val(),
                    IsShow: $("#ChkIsShow").is(':checked') ? "1" : "0",
                    BlogDetailTittle: $("#txtBlogDetailTitle").val(),
                    BlogImage: BlogImage,
                    BlogDetails: blogdata,
                    ImageName: ImageName,
                    ImageExtension: ImageExtension
                },
            success: function (data) {
                data = data.d;
                alert("Save Data Succesfully");
                GetAllBlog();
                Clear();
                if (data.Status == "1" || data.Status == "2") {

                    alert(data.Result);


                } else {
                    alert(data.Result);
                }
                if (data.Focus != "") {
                    $("#" + data.Focus).focus();
                }
            },
            error: function (response) { alert(response.responseText); }
        });
        return false;
    });
}

function ShowBlogDetails() {
    $.ajax({
        url: GlobalURL + "BlogMaster/ShowBlogDetails",
        type: "POST",
        data: {},
        success: function (data) {
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

//function ShowBlogDetailsByBlogID(BlogID) {
//    var url = "../BlogMaster/ShowBlogDetailsByBlogID?BlogID=" + BlogID;
//    window.open(url, '_blank');
//}

function Clear() {
    $("#txtBlogTitle").val("");
    $("#txtBlogDiscription").val("");
    $("#txtBlogSequence").val("");
    $("#txtSubmisionDate").val("");
    $("#ddlBlogCategory").val(0);
    $("#ChkIsShow").prop("checked", false);
    $("#txtBlogDetailTitle").val("");
    $("#fuBlogImage").val("");
    $("#txtBlogFullDetailed").val("");


}

function GetAllBlog() {
    $.ajax({
        url: GlobalURL + "BlogMaster/GetAllBlog",
        type: "POST",
        data: {},
        success: function (data) {
            $("#lbBlogDetails").html(data.Grid);
            $("#totalrecord").html(data.RowCount);
        },
        error: function () {
            alert("Record Not Load");
        }
    })
}

function BlogDataDelete(BlogID) {
    if (confirm("Are you sure to delete!!")) {
        $.ajax({
            url: GlobalURL + "BlogMaster/BlogDataDelete",
            type: "POST",
            data: { BlogID: BlogID },
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

function BlogEditRecord(BlogID) {
    $.ajax({
        url: GlobalURL + "BlogMaster/BlogEditRecord",
        type: "POST",
        data: { BlogID: BlogID },
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

function GetCategoryRecord() {
    $.ajax({
        url: GlobalURL + "BlogMaster/GetCategoryRecord",
        type: "POST",
        data: {},
        success: function (data) {
            data = JSON.parse(data);
            for (var i = 0; i < data.length; i++) {
                //$("#ddlCountry").append($('<option/>').attr("value", data[i].CountryCode).text(data[i].CountryCode));
                $("#ddlBlogCategory").append($('<option value=' + data[i].CatID + '>' + data[i].CategoryName + '</option>'));

            }
        },
        error: function () {
            alert("Category Not Load");
        }
    })
}

function GetImage() {
    $.ajax({
        url: 'BlogMaster/GetImage',
        type: 'post',
        data: {},
        success: function (data) {
            $("#lblImage").html(data.Grid);
        },
        error: function () {
            alert("Image not found");
        }
    });
}




function GetBlogBySearchFilter(BlogTitle) {
    $.ajax({
        url: GlobalURL + "BlogMaster/GetBlogBySearchFilter",
        type: "POST",
        data: { BlogTitle: $("#txtSearch").val() },
        success: function (data) {
            $("#lbPacketDetails").html(data.Grid);

        },
        error: function () {
            alert("Record Not Load");
        }
    })
}





