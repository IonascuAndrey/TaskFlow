document.addEventListener('DOMContentLoaded', () => {
    const draggables = document.querySelectorAll(".task");
    const droppables = document.querySelectorAll(".swim-lane");

    draggables.forEach((task) => {
        task.addEventListener("dragstart", () => {
            console.log("Drag start!");
            task.classList.add("is-dragging");
        });
        task.addEventListener("dragend", () => {
            console.log("Drag end!");
            task.classList.remove("is-dragging");

            const newStatus = task.closest(".swim-lane").id;
            const taskId = task.getAttribute("data-id");

            fetch('/AppTasks/UpdateStatus', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    taskId: taskId,
                    newStatus: newStatus,
                })
            })
                .then(response => response.json())
                .then(data => {
                    console.log("Task updated:", data);
                })
                .catch(error => {
                    console.error('Error updating status:', error);
                });
        });
    });


    droppables.forEach((zone) => {
        zone.addEventListener("dragover", (e) => {
            e.preventDefault();
            const bottomTask = insertAboveTask(zone, e.clientY);
            const curTask = document.querySelector(".is-dragging");

            if (!bottomTask) {
                zone.appendChild(curTask);
            } else {
                zone.insertBefore(curTask, bottomTask);
            }
        });
    });
});


const insertAboveTask = (zone, mouseY) => {
    const els = zone.querySelectorAll(".task:not(.is-dragging)");

    let closestTask = null;
    let closestOffset = Number.NEGATIVE_INFINITY;

    els.forEach((task) => {
        const { top } = task.getBoundingClientRect();
        const offset = mouseY - top;
        if (offset < 0 && offset > closestOffset) {
            closestOffset = offset;
            closestTask = task;
        }
    });
    return closestTask;
};

