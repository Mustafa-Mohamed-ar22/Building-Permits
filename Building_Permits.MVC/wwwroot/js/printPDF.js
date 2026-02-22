function printPDF(filename = "document.pdf") {
    try {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF("p", "mm", "a4");

        // 🔹 Register Arabic font (Amiri Bold from Amiri-Bold-normal.js)
        doc.addFileToVFS("Amiri-Bold.ttf", AmiriBold);
        doc.addFont("Amiri-Bold.ttf", "Amiri-Bold", "normal");
        doc.setFont("Amiri-Bold");

        // Find the element with printPDF class
        const container = document.querySelector(".printPDF");
        if (!container) {
            alert('خطأ: لم يتم العثور على عنصر بكلاس "printPDF"');
            console.error('No element with class "printPDF" found');
            return;
        }

        const pageWidth = doc.internal.pageSize.getWidth();
        const pageHeight = doc.internal.pageSize.getHeight();
        const margin = 15;
        const contentWidth = pageWidth - margin * 2;
        let yPosition = 20;

        // Process container content recursively
        processElement(container, doc, yPosition, margin, contentWidth, pageHeight, pageWidth);

        // Save the PDF
        doc.save(filename);

    } catch (error) {
        console.error("Error generating PDF:", error);
        alert("حدث خطأ أثناء إنشاء ملف PDF: " + error.message);
    }
}

/**
 * Extract text safely, removing unwanted elements.
 */
function getTextContent(element) {
    const clone = element.cloneNode(true);

    // Remove no-print, buttons, and links
    clone.querySelectorAll("button, a, .btn, .bp-btn, .no-print").forEach(el => el.remove());

    return clone.textContent.trim();
}

/**
 * Handle Arabic text (currently no reversal, just natural order).
 */
function reverseArabicText(text) {
    return text; // rely on RTL alignment
}

/**
 * Recursively process DOM elements and render to PDF.
 */
function processElement(element, doc, startY, margin, contentWidth, pageHeight, pageWidth) {
    let yPosition = startY;
    const children = Array.from(element.children);

    for (let child of children) {
        // Skip elements not to print
        if (child.classList.contains("no-print") ||
            child.tagName === "BUTTON" ||
            child.tagName === "A" ||
            child.classList.contains("btn") ||
            child.classList.contains("bp-btn")) {
            continue;
        }

        if (yPosition > pageHeight - 30) {
            doc.addPage();
            yPosition = 20;
        }

        if (child.tagName === "H1") {
            const text = reverseArabicText(getTextContent(child));
            doc.setFontSize(18);
            doc.text(text, pageWidth - margin, yPosition, { align: "right" });
            yPosition += 12;

        } else if (child.tagName === "H2") {
            const text = reverseArabicText(getTextContent(child));
            doc.setFontSize(16);
            doc.text(text, pageWidth - margin, yPosition, { align: "right" });
            yPosition += 10;

        } else if (child.tagName === "H3") {
            const text = reverseArabicText(getTextContent(child));
            doc.setFontSize(14);
            doc.text(text, pageWidth - margin, yPosition, { align: "right" });
            yPosition += 8;

        } else if (child.tagName === "P") {
            const text = reverseArabicText(getTextContent(child));
            if (text) {
                doc.setFontSize(11);
                const lines = doc.splitTextToSize(text, contentWidth);
                lines.forEach(line => {
                    doc.text(line, pageWidth - margin, yPosition, { align: "right" });
                    yPosition += 6;
                });
                yPosition += 4;
            }

        } else if (child.tagName === "TABLE") {
            yPosition = addTableToPDF(doc, child, yPosition, margin, pageHeight, pageWidth);
            yPosition += 5;

        } else if (child.tagName === "DIV") {
            const table = child.querySelector("table");
            if (table) {
                yPosition = addTableToPDF(doc, table, yPosition, margin, pageHeight, pageWidth);
                yPosition += 5;
            } else {
                yPosition = processElement(child, doc, yPosition, margin, contentWidth, pageHeight, pageWidth);
            }

        } else if (child.tagName === "UL" || child.tagName === "OL") {
            const items = child.querySelectorAll("li");
            doc.setFontSize(11);
            items.forEach((item, index) => {
                const text = getTextContent(item);
                const bullet = child.tagName === "UL" ? "•" : `${index + 1}.`;
                const displayText = reverseArabicText(`${bullet} ${text}`);
                const lines = doc.splitTextToSize(displayText, contentWidth - 5);
                lines.forEach(line => {
                    doc.text(line, pageWidth - margin - 5, yPosition, { align: "right" });
                    yPosition += 6;
                });
            });
            yPosition += 3;

        } else {
            const text = reverseArabicText(getTextContent(child));
            if (text && text.trim()) {
                doc.setFontSize(11);
                const lines = doc.splitTextToSize(text, contentWidth);
                lines.forEach(line => {
                    doc.text(line, pageWidth - margin, yPosition, { align: "right" });
                    yPosition += 6;
                });
                yPosition += 3;
            }
        }
    }

    return yPosition;
}

/**
 * Table handler (skips .no-print headers/cells, forces single-line).
 */
function addTableToPDF(doc, tableElement, startY, margin, pageHeight, pageWidth) {
    try {
        const headers = [];
        const rows = [];

        // Headers (skip .no-print)
        const headerCells = tableElement.querySelectorAll("thead th, thead td");
        headerCells.forEach(cell => {
            if (!cell.classList.contains("no-print")) {
                headers.push(getTextContent(cell));
            }
        });

        // Rows (skip .no-print cells)
        const bodyRows = tableElement.querySelectorAll("tbody tr");
        bodyRows.forEach(row => {
            const rowData = [];
            const cells = row.querySelectorAll("td");
            cells.forEach(cell => {
                if (!cell.classList.contains("no-print")) {
                    rowData.push(getTextContent(cell));
                }
            });
            if (rowData.length > 0) {
                rows.push(rowData);
            }
        });

        if (typeof doc.autoTable === "function") {
            const reversedHeaders = headers.map(h => reverseArabicText(h)).reverse();
            const reversedRows = rows.map(row =>
                row.map(cell => reverseArabicText(cell)).reverse()
            );

            doc.autoTable({
                head: [reversedHeaders],
                body: reversedRows,
                startY: startY,
                margin: { left: margin, right: margin },
                styles: {
                    font: "Amiri-Bold",
                    fontSize: 9,
                    cellPadding: 4,
                    halign: "right",
                    overflow: "hidden", // single line
                    cellWidth: "auto"   // no wrapping
                },
                headStyles: {
                    fillColor: [224, 224, 224],
                    fontStyle: "bold",
                    halign: "right"
                },
                alternateRowStyles: {
                    fillColor: [250, 250, 250]
                }
            });

            return doc.lastAutoTable.finalY + 5;
        } else {
            return startY + 10;
        }
    } catch (error) {
        console.error("Error adding table to PDF:", error);
        return startY + 10;
    }
}

console.log("✅ PDF Print Library with Amiri Bold, RTL, .no-print support, and single-line tables loaded");
