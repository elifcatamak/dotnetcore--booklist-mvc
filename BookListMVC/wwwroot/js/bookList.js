var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    // .DataTable populates a data table on DT_load
    dataTable = $('#DT_load').DataTable({
        // Ajax call
        "ajax": {
            "url": "/books/getall",
            "type": "GET",
            "datatype": "json"
        },
        // Defining columns that we display
        // Have to be camel case
        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "author", "width": "20%" },
            { "data": "isbn", "width": "20%" },
            {
                // When we edit or delete, we need to pass the id of the book
                "data": "id",
                // We want to render two buttons here, function returns a div with two buttons
                "render": function (data) {
                    return `<div class="text-center">
                        <a href="/Books/Upsert?id=${data}" class="btn btn-success text-white" style="cursor:pointer; width:70px">
                            Edit
                        </a>
                        &nbsp;
                        <a class="btn btn-danger text-white" style="cursor:pointer; width:70px;"
                            onclick=Delete('books/Delete?id=${data}')>
                            Delete
                        </a>
                    </div>`;
                },
                "width": "40%"
            }
        ],
        "language": {
            "emptyTable": "no data found"
        },
        "width": "100%"
    });
}

function Delete(deleteUrl) {
    // Displaying SweetAlert
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        // willDelete is the user response

        // If user selected yes
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: deleteUrl,
                // When request is successful and response is received (status 2xx)
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}