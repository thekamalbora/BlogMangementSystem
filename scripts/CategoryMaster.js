var GlobalURL = "http://localhost:17197/";
$(document).ready()
{
    
    GetAllCategory();
    $("#btnAddCategory").on("click", function ()
    {
        $.ajax({
            url: GlobalURL + "BlogMaster/CategoryMasterInsertUpdateData",
            type: "POST",
            data:
                {
                    CatID: $("#hfCategoryID").val(),
                    CategoryName: $("#txtCategoryName").val(),
                    Sequence: $("#txtCategorySequence").val(),
                },
            success: function (data)
            {
                data = data.d;
                alert("Save Data Succesfully");
                GetAllCategory();
                Clear();
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
    
    });
}

function Clear()
{
    $("#txtCategoryName").val("");
    $("#txtCategorySequence").val("");

}

function ShowCategories()
{
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

function GetAllCategory() {
    $.ajax({
        url: GlobalURL + "BlogMaster/GetAllCategory",
        type: "POST",
        data: {},
        success: function (data) {
            $("#lbCategoryDetails").html(data.Grid);
            $("#totalrecord").html(data.RowCount);
        },
        error: function () {
            alert("Record Not Load");
        }
    })
}

function CategoryDataDelete(CatID, Sequence) {
    if (confirm("Are you sure to delete!!")) {
        $.ajax({
            url: GlobalURL + "BlogMaster/CategoryDataDelete",
            type: "POST",
            data: { CatID: CatID ,Sequence:Sequence},
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

function CategoryEditRecord(CatID) {
    $.ajax({
        url: GlobalURL + "BlogMaster/CategoryEditRecord",
        type: "POST",
        data: { CatID: CatID },
        success: function (data) {
            data = JSON.parse(data);
            $("#hfCategoryID").val(CatID);
            $("#txtCategoryName").val(data[0].CategoryName);
            $("#txtCategorySequence").val(data[0].Sequence);
            $("#btnPacketSave").val("Update");
        },
        error: function () {
            alert("Record not Edited");
        }
    });
}




