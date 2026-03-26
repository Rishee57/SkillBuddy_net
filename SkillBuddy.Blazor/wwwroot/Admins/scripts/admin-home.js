window.adminHome = {
    toggleSidebar: function(collapsed) {
        const sb = document.getElementById('sidebar');
        if (!sb) return;
        if (collapsed) {
            sb.classList.add('collapsed');
        } else {
            sb.classList.remove('collapsed');
        }
    },
    initCharts: function() {
        if (typeof Chart === 'undefined') { console.warn('Chart.js not loaded.'); return; }

        var ctx = document.getElementById('mainChart').getContext('2d');
        window.mainChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug'],
                datasets: [
                    { label: 'Emails', data: [30,45,28,80,99,43,55,70], backgroundColor: 'rgba(54,162,235,0.6)' },
                    { label: 'Clicks', data: [20,35,18,60,79,33,45,50], backgroundColor: 'rgba(255,99,132,0.6)' },
                    { label: 'Signups', data: [10,25,12,40,50,23,30,35], backgroundColor: 'rgba(255,206,86,0.6)' }
                ]
            },
            options: { responsive: true, plugins: { legend: { position: 'top' } }, scales: { y: { beginAtZero: true } } }
        });

        var ctx2 = document.getElementById('areaChart').getContext('2d');
        window.areaChart = new Chart(ctx2, {
            type: 'line',
            data: { labels: ['Mon','Tue','Wed','Thu','Fri','Sat','Sun'], datasets: [{ label: 'Revenue', data: [120,150,170,160,180,200,220], fill: true, tension: 0.4, backgroundColor: 'rgba(255,193,7,0.2)', borderColor: 'rgba(255,193,7,1)' }] },
            options: { responsive:true, plugins:{ legend:{ display:false } }, scales:{ y:{ beginAtZero:true } } }
        });

        function makeSpark(id, data, color){
            var el = document.getElementById(id); if (!el) return;
            new Chart(el.getContext('2d'), {
                type: 'line',
                data: { labels: data.map((_,i)=>i+1), datasets:[{ data: data, borderColor: color, borderWidth:1, pointRadius:0 }] },
                options: { responsive:false, maintainAspectRatio:false, scales:{ x:{ display:false }, y:{ display:false } }, plugins:{ legend:{ display:false } }, elements:{ line:{ tension:0.4 } } }
            });
        }
        makeSpark('spark1',[3,6,4,8,6,9,7],'rgba(54,162,235,1)');
        makeSpark('spark2',[2,4,3,5,4,6,5],'rgba(255,99,132,1)');
        makeSpark('spark3',[4,5,6,7,6,5,4],'rgba(255,206,86,1)');
    }
};

document.addEventListener('DOMContentLoaded', function(){
    var btn = document.getElementById('sidebarToggle');
    if (btn) btn.addEventListener('click', ()=>{ document.getElementById('sidebar')?.classList.toggle('collapsed'); });
    setTimeout(()=>{ try{ window.adminHome.initCharts(); }catch(e){console.error(e);} }, 250);
});
