﻿// Call the dataTables jQuery plugin
$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var firstNameInput = ($('#first-name-input').val());
        var lastNameInput = ($('#last-name-input').val());
        var privilegeInput = $('#privilege-select option:selected').text();
        var documentInput = ($('#document-input').val());

        
        var firstN = String(data[0]);
        var lastN = String(data[1]);
        var privilege = String(data[3]);
        var document = String(data[4]);

        if (((firstNameInput && firstN.includes(firstNameInput)) || !firstNameInput) &&

            ((lastNameInput && lastN.includes(lastNameInput)) || !lastNameInput) &&

            ((privilegeInput && privilege.includes(privilegeInput)) || !privilegeInput) &&

            ((documentInput&& document.includes(documentInput)) || !documentInput))
        {
            return true;
        }

        return false;
    }
);

$(document).ready(function () {
    $.noConflict();
    var table = $('#dataTable').DataTable({
        columnDefs: [
            { orderable: false, targets: -1 }
        ],
        processing: true,
        serverSide: true,
        order: [[1, "desc"]],
        ajax: {
            url: 'User/GetCurrentPage',
            datatype: 'json',
            type: 'POST'
        },

        //Columns data order       
        columns: [
            { data: "firstName" },
            { data: "lastName" },
            {
                data: "dateOfBirth",
                render: function (data, type, row) {
                    if (data != null) {
                        var date = new Date(Date.parse(data));
                        return date.toLocaleDateString();
                    }
                }
            },
            {
                data: "privilege",
                defaultContent: "",
                render: function (data, type, row) {
                    if (data != null) {
                        return '<a href = "Privileges/Details/'+ data.id +'">' + data.name + '</a>'
                    }
                }
            },
            {
                data: "document",
                defaultContent: "",
                render: function (data, type, row) {
                    if (data != null) {
                        return '<a href = "Documents/Details/' + data.id + '">' + data.number + '</a>'
                    }
                }
            },
            {
                data: null,
                //Set default buttons (Edit, Delete)
                //href = "#" because <a> without href have a special style
                defaultContent:
                    '<a class="btn btn-warning btn-sm" href = "#" id = "editButton">Edit</a>' + ' '
                    + '<a class="btn btn-info btn-sm" href = "#" id = "detailsButton">Details</a>' + ' '
                    + '<a class="btn btn-danger btn-sm" href = "#" id = "deleteButton">Delete</a>'
            }
        ],
        language: {
            //Set message for pop-up window
            processing: "Take data from server. Please wait..."
        }
    });

    //Change event listener for search
    //Search after pressing Enter or defocusing the search input field
    $("#dataTable_filter input").unbind()
        .bind("change", function (e) {
            var searchValue = $(this).val();
            table.search(searchValue).draw();
        });

    $("#dataTable tbody").on('click', '#editButton', function () {
        var data = table.row($(this).parents('tr')).data();
        location.href = "/User/Edit/" + data.id;
    })
    //Event listener for Details button 
    $("#dataTable tbody").on('click', '#detailsButton', function () {
        var data = table.row($(this).parents('tr')).data();
        location.href = "/User/Details/" + data.id;
    })
    //Event listener for Delete button 
    $("#dataTable tbody").on('click', '#deleteButton', function () {
        var data = table.row($(this).parents('tr')).data();
        location.href = "/User/Delete/" + data.id;
    })

    $('#first-name-input, #last-name-input, #privilege-select, #document-input').keyup(function () {
        table.draw();
    });

    $('#privilege-select').change(function () {
        table.draw();
    });

    //Delete container from loyout only for Index
    $('.container').removeClass('container');

});
