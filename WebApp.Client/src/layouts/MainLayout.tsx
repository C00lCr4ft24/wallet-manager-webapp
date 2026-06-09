import TopNavBar from '@/components/TopNavBar';
import { Outlet } from 'react-router-dom';
import { Toaster } from 'sonner';

function MainLayout() {
  return (
    <>
        <TopNavBar />
        <div className="flex-1 overflow-auto">
          <Outlet />
        </div>
        <Toaster position="bottom-center" />
    </>
  )
}

export default MainLayout