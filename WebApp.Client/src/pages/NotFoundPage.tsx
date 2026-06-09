
import { Button } from '@/components/ui/button'
import { useNavigate } from 'react-router-dom'

const NotFoundPage = () => {
	const navigate = useNavigate()

  return (
    <div className="flex flex-col items-center justify-center h-screen">
      <h1 className="text-4xl font-bold mb-4">404 - Not Found</h1>
        <p className="text-lg text-gray-600">The page you are looking for does not exist.</p>
        <Button className="mt-6" onClick={() => navigate("/")}>
          Go Back
        </Button>
    </div>
  )
}

export default NotFoundPage