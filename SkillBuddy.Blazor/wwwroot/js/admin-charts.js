// Admin Dashboard Charts - Matching Uploaded Design

document.addEventListener('DOMContentLoaded', function() {
    
    // Chart.js default configuration
    Chart.defaults.global = {
        ...Chart.defaults.global,
        defaultFontFamily: 'Nunito, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif',
        defaultFontColor: '#858796'
    };

    // Small Charts for Top Cards
    function createSmallChart(canvasId, color, data) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;
        
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                datasets: [{
                    data: data,
                    backgroundColor: 'transparent',
                    borderColor: color,
                    borderWidth: 2,
                    pointRadius: 0,
                    pointHitRadius: 10,
                    pointHoverRadius: 4,
                    pointHoverBackgroundColor: color,
                    pointHoverBorderColor: '#ffffff',
                    pointHoverBorderWidth: 2,
                }]
            },
            options: {
                maintainAspectRatio: false,
                layout: {
                    padding: {
                        left: 10,
                        right: 10,
                        top: 5,
                        bottom: 5
                    }
                },
                scales: {
                    x: {
                        display: false,
                        grid: {
                            display: false
                        }
                    },
                    y: {
                        display: false,
                        grid: {
                            display: false
                        }
                    }
                },
                elements: {
                    line: {
                        tension: 0.4
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        enabled: false
                    }
                }
            }
        });
    }

    // Create small charts for the top cards
    createSmallChart('blueChart', '#4e73df', [5, 10, 15, 8, 20, 25]);
    createSmallChart('pinkChart', '#e74a3b', [8, 15, 10, 18, 12, 22]);
    createSmallChart('orangeChart', '#f6c23e', [12, 8, 16, 20, 15, 28]);
    createSmallChart('greenChart', '#1cc88a', [6, 12, 18, 14, 22, 30]);

    // Main Chart (Revenue Analytics)
    const mainChartCtx = document.getElementById('mainChart');
    if (mainChartCtx) {
        new Chart(mainChartCtx, {
            type: 'bar',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'],
                datasets: [{
                    label: 'Revenue',
                    data: [4215, 5312, 6251, 7841, 9821, 14984, 12345],
                    backgroundColor: '#4e73df',
                    borderColor: '#4e73df',
                    borderWidth: 1,
                    borderRadius: 4,
                    borderSkipped: false,
                }]
            },
            options: {
                maintainAspectRatio: false,
                layout: {
                    padding: {
                        left: 10,
                        right: 25,
                        top: 25,
                        bottom: 0
                    }
                },
                scales: {
                    x: {
                        grid: {
                            display: false,
                            drawBorder: false
                        },
                        ticks: {
                            maxTicksLimit: 7
                        }
                    },
                    y: {
                        ticks: {
                            maxTicksLimit: 5,
                            padding: 10,
                            callback: function(value) {
                                return '$' + value.toLocaleString();
                            }
                        },
                        grid: {
                            color: "rgb(234, 236, 244)",
                            zeroLineColor: "rgb(234, 236, 244)",
                            drawBorder: false,
                            borderDash: [2],
                            zeroLineBorderDash: [2]
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: "rgb(255,255,255)",
                        bodyColor: "#858796",
                        titleColor: '#6e707e',
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        displayColors: false
                    }
                }
            }
        });
    }

    // Pie Chart (Revenue Source)
    const pieChartCtx = document.getElementById('pieChart');
    if (pieChartCtx) {
        new Chart(pieChartCtx, {
            type: 'doughnut',
            data: {
                labels: ['Direct', 'Referral', 'Social'],
                datasets: [{
                    data: [55, 30, 15],
                    backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc'],
                    hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf'],
                    hoverBorderColor: "rgba(234, 236, 244, 1)",
                }]
            },
            options: {
                maintainAspectRatio: false,
                plugins: {
                    tooltip: {
                        backgroundColor: "rgb(255,255,255)",
                        bodyColor: "#858796",
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        displayColors: false
                    },
                    legend: {
                        display: false
                    }
                },
                cutout: '80%'
            }
        });
    }

    // Bottom Chart (New York card)
    const bottomChartCtx = document.getElementById('bottomChart');
    if (bottomChartCtx) {
        new Chart(bottomChartCtx, {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                datasets: [{
                    data: [5, 10, 15, 8, 12, 18, 20, 15, 25, 22, 30, 28],
                    backgroundColor: 'rgba(255, 255, 255, 0.1)',
                    borderColor: 'rgba(255, 255, 255, 0.8)',
                    borderWidth: 2,
                    pointRadius: 0,
                    pointHitRadius: 10,
                    pointHoverRadius: 4,
                    pointHoverBackgroundColor: 'rgba(255, 255, 255, 1)',
                    pointHoverBorderColor: 'rgba(255, 255, 255, 1)',
                    pointHoverBorderWidth: 2,
                    fill: true
                }]
            },
            options: {
                maintainAspectRatio: false,
                layout: {
                    padding: {
                        left: 10,
                        right: 10,
                        top: 10,
                        bottom: 10
                    }
                },
                scales: {
                    x: {
                        display: false,
                        grid: {
                            display: false
                        }
                    },
                    y: {
                        display: false,
                        grid: {
                            display: false
                        }
                    }
                },
                elements: {
                    line: {
                        tension: 0.4
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        enabled: false
                    }
                }
            }
        });
    }

    // Add animation to cards
    const cards = document.querySelectorAll('.card');
    cards.forEach((card, index) => {
        card.style.animationDelay = `${index * 0.1}s`;
    });
});

// Utility functions for future enhancements
function updateChartData(chartId, newData) {
    const chart = Chart.getChart(chartId);
    if (chart) {
        chart.data.datasets[0].data = newData;
        chart.update();
    }
}

function resizeCharts() {
    Chart.helpers.each(Chart.instances, function(instance) {
        instance.resize();
    });
}

// Handle window resize
window.addEventListener('resize', resizeCharts);