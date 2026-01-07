document.getElementById("button-search").addEventListener("click", function (e) {
    e.preventDefault();

    const searchBy = document.querySelector('[name="searchBy"]').value;
    const searchString = document.querySelector('[name="searchString"]').value;

    fetch(`/Persons/SearchTable?searchby=${searchBy}&searchstring=${searchString}`)
        .then(res => res.text())
        .then(html => {
            document.getElementById("persons-table-body").innerHTML = html;
        });
})