import { useNavigate } from "react-router-dom";
import { APP_NAME } from "../lib/consts";
import { toast } from "sonner";
import { Button } from "./ui/button";
import UserAvatarItem from "./UserAvatarItem";
import { useEffect, useState } from "react";
import { ProblemDetails } from "@/generated/dtos";
import userService from "@/services/userService";


function TopNavBar() {
  const [userId, setUserId] = useState<number>(-1)
  const [userName, setUserName] = useState<string>('')

  const navigate = useNavigate()

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const user = await userService.currentUser()
        setUserId(user.id)
        setUserName(user.userName)
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      }
    }
    fetchUser()
  }, [])

  const handleLogout = () => {
    userService.logout()
    toast.success("Logged out successfully!")
    navigate('/login')
  }

  return (
    <nav className="bg-black text-white p-4">
      <div className="container mx-auto flex items-center justify-between">
        <Button variant="ghost" className="text-lg font-bold" onClick={() => navigate('/dashboard')}>
          {APP_NAME}
        </Button>
        <div>
          <Button variant="ghost" onClick={() => navigate('/dashboard')}> Dashboard </Button>
          <Button variant="ghost" onClick={() => navigate('/family')}> Family </Button>
          <Button variant="ghost" onClick={() => navigate('/wallets')}> Wallets </Button>
          <Button variant="ghost" onClick={() => navigate('/transactions')}> Transactions </Button>
          <Button variant="ghost" onClick={() => navigate('/categories')}> Categories </Button>
          <Button variant="ghost" onClick={() => navigate('/limits')}> Limits </Button>
          <Button variant="ghost" onClick={() => navigate('/profile')}> Profile </Button>
          <Button variant="ghost" onClick={handleLogout}> Logout </Button>
          <Button variant="ghost">
            <UserAvatarItem userId={userId} userName={userName} />
          </Button>
        </div>
      </div>
    </nav>
  )
}

export default TopNavBar